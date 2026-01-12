import asyncio
import websockets
import json
import os

# --- DATABASE CONFIG ---
DB_FILE = "users.json"
CLIENTS = {} 

# --- HELPER FUNCTIONS ---
def load_users():
    if not os.path.exists(DB_FILE): return {}
    try:
        with open(DB_FILE, "r") as f: return json.load(f)
    except: return {}

def save_users(users_data):
    with open(DB_FILE, "w") as f: json.dump(users_data, f, indent=4)

async def broadcast_to_room(sender_ws, data):
    sender_info = CLIENTS.get(sender_ws)
    if not sender_info or not sender_info["room"]: return
    target_room = sender_info['room']
    message = json.dumps(data)
    for ws, info in CLIENTS.items():
        if info["room"] == target_room and ws != sender_ws:
            try: await ws.send(message)
            except: pass

async def handler(websocket):
    print(f"[NEW] Connection from {websocket.remote_address}")
    CLIENTS[websocket] = {"room": None, "username": None}

    try:
        async for message in websocket:
            # --- ΤΡΟΠΟΠΟΙΗΣΗ: ΕΚΤΥΠΩΣΗ ΤΟΥ ΜΗΝΥΜΑΤΟΣ ---
            print(f"[RECEIVED] {message}") 
            # -------------------------------------------
            
            try:
                data = json.loads(message)
                msg_type = data.get("type")
                
                # --- REGISTER ---
                if msg_type == "register":
                    u, p, e = data.get("username"), data.get("password"), data.get("email", "")
                    users_db = load_users()
                    if u in users_db:
                        await websocket.send(json.dumps({"type": "register_response", "status": "fail", "msg": "Username taken"}))
                    else:
                        # Initialize with empty friends list AND requests list
                        users_db[u] = {"password": p, "email": e, "friends": [], "requests": []}
                        save_users(users_db)
                        CLIENTS[websocket]["username"] = u
                        await websocket.send(json.dumps({"type": "register_response", "status": "success", "username": u}))

                # --- LOGIN ---
                elif msg_type == "login":
                    u, p = data.get("username"), data.get("password")
                    users_db = load_users()
                    if u in users_db and users_db[u]["password"] == p:
                        CLIENTS[websocket]["username"] = u
                        
                        # Data Migration: Ensure lists exist
                        if "friends" not in users_db[u]: users_db[u]["friends"] = []
                        if "requests" not in users_db[u]: users_db[u]["requests"] = []
                        save_users(users_db)
                        
                        friends_list = users_db[u]["friends"]
                        requests_list = users_db[u]["requests"]
                        
                        await websocket.send(json.dumps({
                            "type": "login_response", 
                            "status": "success", 
                            "username": u,
                            "friends": friends_list,
                            "requests": requests_list
                        }))
                    else:
                        await websocket.send(json.dumps({"type": "login_response", "status": "fail", "msg": "Invalid credentials"}))

                # --- SEARCH USER ---
                elif msg_type == "search_user":
                    query = data.get("query")
                    users_db = load_users()
                    if query in users_db:
                        await websocket.send(json.dumps({"type": "search_response", "status": "found", "username": query}))
                    else:
                        await websocket.send(json.dumps({"type": "search_response", "status": "not_found"}))

                # --- SEND FRIEND REQUEST ---
                elif msg_type == "send_request":
                    me = CLIENTS[websocket]["username"] or data.get("username")
                    target = data.get("target_user")
                    users_db = load_users()

                    if target in users_db and target != me:
                        # 1. Check if already friends
                        if target in users_db[me].get("friends", []):
                            await websocket.send(json.dumps({"type": "error", "msg": "Already friends"}))
                        # 2. Check if request already sent
                        elif me in users_db[target].get("requests", []):
                            await websocket.send(json.dumps({"type": "error", "msg": "Request already sent"}))
                        else:
                            # Add to target's pending requests
                            if "requests" not in users_db[target]: users_db[target]["requests"] = []
                            users_db[target]["requests"].append(me)
                            save_users(users_db)

                            print(f"[LOG] {me} sent request to {target}")
                            await websocket.send(json.dumps({"type": "request_sent", "target": target}))
                            
                            # Notify target if online
                            for ws, info in CLIENTS.items():
                                if info["username"] == target:
                                    await ws.send(json.dumps({"type": "new_request", "from": me}))
                                    break
                    else:
                        await websocket.send(json.dumps({"type": "error", "msg": "User not found"}))

                # --- RESPOND TO REQUEST (Accept/Reject) ---
                elif msg_type == "respond_request":
                    me = CLIENTS[websocket]["username"] or data.get("username")
                    requester = data.get("requester")
                    action = data.get("action") # "accept" or "reject"
                    
                    users_db = load_users()
                    
                    if me in users_db and requester in users_db[me].get("requests", []):
                        # Remove from requests
                        users_db[me]["requests"].remove(requester)
                        
                        if action == "accept":
                            # Add mutual friendship
                            if "friends" not in users_db[me]: users_db[me]["friends"] = []
                            if "friends" not in users_db[requester]: users_db[requester]["friends"] = []
                            
                            if requester not in users_db[me]["friends"]: users_db[me]["friends"].append(requester)
                            if me not in users_db[requester]["friends"]: users_db[requester]["friends"].append(me)
                            
                            save_users(users_db)
                            print(f"[LOG] {me} accepted {requester}")

                            # Notify ME (Updated Friends List)
                            await websocket.send(json.dumps({
                                "type": "friends_list", 
                                "friends": users_db[me]["friends"],
                                "requests": users_db[me]["requests"]#s Στέλνουμε όλο το πακέτο
                            }))
                            
                            # Notify REQUESTER (They are online?)
                            for ws, info in CLIENTS.items():
                                if info["username"] == requester:
                                    await ws.send(json.dumps({
                                        "type": "friends_list",#Στέλνουμε το update με το σωστό type
                                        "friends": users_db[requester]["friends"],
                                        "requests": users_db[requester]["requests"]
                                    }))
                                    break
                        else:
                            # Reject: Just save the removal from requests
                            save_users(users_db)
                            print(f"[LOG] {me} rejected {requester}")
                            # Στέλνουμε πίσω τις ανανεωμένες λίστες requests
                            await websocket.send(json.dumps({
                                "type": "friends_list", 
                                "friends": users_db[me]["friends"],
                                "requests": users_db[me]["requests"] 
                            }))

                # --- GET FRIENDS & REQUESTS ---
                elif msg_type == "get_friends":
                    u = data.get("username") or CLIENTS[websocket]["username"]
                    users_db = load_users()
                    if u in users_db:
                        await websocket.send(json.dumps({
                            "type": "friends_list",
                            "friends": users_db[u].get("friends", []),
                            "requests": users_db[u].get("requests", [])
                        }))

                # --- JOIN / CHAT ---
                elif msg_type == "join":
                    CLIENTS[websocket]["room"] = data.get("room", "Lobby")
                    if data.get("username"): CLIENTS[websocket]["username"] = data.get("username")

                elif msg_type in ["chat", "gesture"]:
                    data["sender"] = CLIENTS[websocket]["username"]
                    await broadcast_to_room(websocket, data)

            except json.JSONDecodeError: pass
    except websockets.exceptions.ConnectionClosed: pass
    finally:
        if websocket in CLIENTS: del CLIENTS[websocket]

async def main():
    print("--- SERVER WITH FRIEND REQUESTS STARTED ---")
    async with websockets.serve(handler, "localhost", 8765):
        await asyncio.Future()

if __name__ == "__main__":
    asyncio.run(main())