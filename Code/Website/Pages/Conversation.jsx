import React, { useState, useEffect, useRef } from 'react';
import { base44 } from '@/api/base44Client';
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { motion, AnimatePresence } from 'framer-motion';
import { 
  Mic, Send, Video, Keyboard, Hand, 
  RefreshCw, Settings2, Camera, User, Copy, Check
} from 'lucide-react';
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Tabs, TabsList, TabsTrigger, TabsContent } from "@/components/ui/tabs";
import { Badge } from "@/components/ui/badge";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import ChatBubble from '../components/chat/ChatBubble';

// Placeholder image for the 3D Avatar view
const AVATAR_PLACEHOLDER = "https://qtrypzzcjebvfcihiynt.supabase.co/storage/v1/object/public/base44-prod/public/user_69316d42bead069e4794658c/a85efe8a2_image.png";

export default function Conversation() {
  const [activeRole, setActiveRole] = useState('speaker'); // 'speaker' or 'signer'
  const [inputText, setInputText] = useState("");
  const scrollRef = useRef(null);
  const queryClient = useQueryClient();

  // Fetch messages
  const { data: messages, isLoading } = useQuery({
    queryKey: ['messages'],
    queryFn: () => base44.entities.Message.list({ sort: { created_date: 1 } }),
    initialData: [],
    refetchInterval: 2000, // Polling for real-time feel
  });

  // Send message mutation
  const sendMessageMutation = useMutation({
    mutationFn: (newMsg) => base44.entities.Message.create(newMsg),
    onSuccess: () => {
      queryClient.invalidateQueries(['messages']);
      setInputText("");
    }
  });

  // Auto-scroll to bottom
  useEffect(() => {
    if (scrollRef.current) {
      scrollRef.current.scrollTop = scrollRef.current.scrollHeight;
    }
  }, [messages]);

  const handleSend = (e) => {
    e.preventDefault();
    if (!inputText.trim()) return;

    sendMessageMutation.mutate({
      content: inputText,
      sender: activeRole,
      type: activeRole === 'signer' ? 'sign' : 'text'
    });
  };

  // Mock gesture detection for Signer mode
  const triggerGesture = (gestureName) => {
    sendMessageMutation.mutate({
      content: `[GESTURE: ${gestureName.toUpperCase()}]`,
      sender: 'signer',
      type: 'sign'
    });
  };

  return (
    <div className="grid grid-cols-1 lg:grid-cols-12 gap-6 h-[calc(100vh-8rem)]">
      
      {/* LEFT PANEL: Visual Context (Avatar or Camera) */}
      <div className="lg:col-span-7 flex flex-col gap-4">
        <div className="bg-slate-900 rounded-2xl border border-slate-800 overflow-hidden relative flex-1 shadow-2xl">
          {/* Header Overlay */}
          <div className="absolute top-0 left-0 right-0 p-4 bg-gradient-to-b from-black/80 to-transparent z-10 flex justify-between items-start">
            <div>
              <h3 className="text-white font-semibold flex items-center gap-2">
                {activeRole === 'speaker' ? (
                  <><Video className="w-4 h-4 text-indigo-400" /> Sign Output Feed</>
                ) : (
                  <><Camera className="w-4 h-4 text-green-400" /> Input Recognition</>
                )}
              </h3>
              <p className="text-xs text-slate-300 mt-1">
                {activeRole === 'speaker' 
                  ? "Visualizing text-to-sign translation" 
                  : "Detecting hand gestures via camera"}
              </p>
            </div>
            <Badge variant="outline" className="bg-black/50 border-slate-600 backdrop-blur-md text-white">
              <div className={`w-2 h-2 rounded-full mr-2 ${activeRole === 'speaker' ? 'bg-indigo-500' : 'bg-green-500'} animate-pulse`} />
              Live
            </Badge>
          </div>

          {/* Main Visual Area */}
          <div className="absolute inset-0 flex items-center justify-center bg-slate-950">
             {activeRole === 'speaker' ? (
               // Speaker View: Sees the Avatar signing
               <div className="relative w-full h-full">
                 <img 
                   src={AVATAR_PLACEHOLDER} 
                   alt="Sign Avatar" 
                   className="w-full h-full object-cover opacity-80"
                 />
                 <div className="absolute bottom-8 left-1/2 -translate-x-1/2 bg-black/60 backdrop-blur-xl px-6 py-3 rounded-full text-white font-medium border border-white/10">
                   Waiting for input...
                 </div>
               </div>
             ) : (
               // Signer View: Sees themselves (Webcam mock)
               <div className="relative w-full h-full bg-slate-900 flex flex-col items-center justify-center">
                 <div className="w-full h-full absolute inset-0 opacity-20 bg-[url('https://images.unsplash.com/photo-1526374965328-7f61d4dc18c5?auto=format&fit=crop&q=80')] bg-cover bg-center" />
                 <div className="z-10 flex flex-col items-center gap-4">
                   <div className="w-24 h-24 rounded-full border-4 border-green-500/30 flex items-center justify-center animate-pulse">
                     <Hand className="w-10 h-10 text-green-500" />
                   </div>
                   <p className="text-slate-400">Camera Active • Tracking Hands</p>
                   
                   {/* Quick Gesture Debugging Buttons */}
                   <div className="flex gap-2 mt-4">
                     <Button size="sm" variant="secondary" onClick={() => triggerGesture('Hello')}>👋 Hello</Button>
                     <Button size="sm" variant="secondary" onClick={() => triggerGesture('Thank You')}>🙏 Thanks</Button>
                     <Button size="sm" variant="secondary" onClick={() => triggerGesture('Yes')}>👍 Yes</Button>
                   </div>
                 </div>
               </div>
             )}
          </div>
        </div>

        {/* Role Switcher Toggle */}
        <div className="bg-slate-900 p-2 rounded-xl border border-slate-800 flex items-center gap-2">
          <span className="text-sm text-slate-400 px-3">View Mode:</span>
          <Tabs value={activeRole} onValueChange={setActiveRole} className="w-full">
            <TabsList className="w-full bg-slate-950 border border-slate-800">
              <TabsTrigger value="speaker" className="w-1/2 data-[state=active]:bg-indigo-600 data-[state=active]:text-white">
                <User className="w-4 h-4 mr-2" /> Speaker (Hearing)
              </TabsTrigger>
              <TabsTrigger value="signer" className="w-1/2 data-[state=active]:bg-green-600 data-[state=active]:text-white">
                <Hand className="w-4 h-4 mr-2" /> Signer (Deaf/HoH)
              </TabsTrigger>
            </TabsList>
          </Tabs>
        </div>
      </div>

      {/* RIGHT PANEL: Chat Interface */}
      <div className="lg:col-span-5 flex flex-col bg-slate-900 rounded-2xl border border-slate-800 shadow-xl overflow-hidden">
        {/* Chat Header */}
        <div className="p-4 border-b border-slate-800 bg-slate-900/50 flex justify-between items-center">
          <div>
            <h2 className="font-semibold text-white">Conversation</h2>
            <p className="text-xs text-slate-400">Session ID: #8291-A</p>
          </div>
          <div className="flex gap-2">
             <Button variant="ghost" size="sm" className="text-slate-400 hover:text-white" onClick={() => queryClient.invalidateQueries(['messages'])}>
               <RefreshCw className="w-4 h-4 mr-2" /> Sync
             </Button>
             
             <Dialog>
               <DialogTrigger asChild>
                 <Button size="sm" className="bg-indigo-600 hover:bg-indigo-500 text-white border-0">
                   <Settings2 className="w-4 h-4 mr-2" /> Connect Unity
                 </Button>
               </DialogTrigger>
               <DialogContent className="bg-slate-900 border-slate-800 text-white max-w-2xl">
                 <DialogHeader>
                   <DialogTitle className="text-xl">🔌 Connect Your Unity App</DialogTitle>
                   <DialogDescription className="text-slate-400">
                     Follow these steps to link your Unity project to this dashboard.
                   </DialogDescription>
                 </DialogHeader>
                 
                 <div className="space-y-6 mt-4">
                   {/* Step 1 */}
                   <div className="space-y-2">
                     <h4 className="text-sm font-medium text-white flex items-center gap-2">
                       <span className="flex items-center justify-center w-5 h-5 rounded-full bg-indigo-600 text-[10px] font-bold">1</span>
                       Copy this API URL
                     </h4>
                     <div className="flex gap-2">
                       <code className="flex-1 bg-slate-950 p-3 rounded-lg border border-slate-800 text-sm font-mono text-green-400 break-all">
                         {window.location.origin}/api/entities/Message
                       </code>
                       <Button size="icon" variant="outline" className="border-slate-700 shrink-0 hover:bg-slate-800"
                         onClick={() => {
                           navigator.clipboard.writeText(`${window.location.origin}/api/entities/Message`);
                         }}
                       >
                         <Copy className="w-4 h-4" />
                       </Button>
                     </div>
                     <p className="text-xs text-slate-500">
                       Paste this into the <code>apiUrl</code> field in the Unity script I provided.
                     </p>
                   </div>

                   {/* Step 2 */}
                   <div className="space-y-2">
                     <h4 className="text-sm font-medium text-white flex items-center gap-2">
                       <span className="flex items-center justify-center w-5 h-5 rounded-full bg-indigo-600 text-[10px] font-bold">2</span>
                       How it works
                     </h4>
                     <div className="grid grid-cols-2 gap-4">
                       <div className="p-3 rounded-lg bg-slate-950 border border-slate-800">
                         <div className="text-xs font-bold text-slate-500 mb-1 uppercase">Unity → Web</div>
                         <div className="text-sm text-slate-300">
                           Sends <b>POST</b> requests when you make a gesture.
                         </div>
                       </div>
                       <div className="p-3 rounded-lg bg-slate-950 border border-slate-800">
                         <div className="text-xs font-bold text-slate-500 mb-1 uppercase">Web → Unity</div>
                         <div className="text-sm text-slate-300">
                           Unity <b>GETs</b> (polls) this URL every second to check for new text.
                         </div>
                       </div>
                     </div>
                   </div>
                 </div>
               </DialogContent>
             </Dialog>
          </div>
        </div>

        {/* Messages Area */}
        <div className="flex-1 overflow-y-auto p-4 space-y-4 bg-slate-950/30" ref={scrollRef}>
          {messages.map((msg) => (
            <ChatBubble 
              key={msg.id} 
              message={msg} 
              isMe={msg.sender === activeRole} 
            />
          ))}
          {isLoading && <div className="text-center text-xs text-slate-500 mt-4">Syncing history...</div>}
        </div>

        {/* Input Area */}
        <div className="p-4 bg-slate-900 border-t border-slate-800">
          <form onSubmit={handleSend} className="flex gap-2 items-end">
            <div className="flex-1 relative">
              <Input 
                className="bg-slate-950 border-slate-700 text-slate-200 pr-10 focus-visible:ring-indigo-500"
                placeholder={activeRole === 'speaker' ? "Type message to translate..." : "Recognized text will appear here..."}
                value={inputText}
                onChange={(e) => setInputText(e.target.value)}
                disabled={activeRole === 'signer'} // Signer input comes from gestures
              />
              <div className="absolute right-2 top-1/2 -translate-y-1/2 text-slate-500">
                {activeRole === 'speaker' ? <Keyboard className="w-4 h-4" /> : <Hand className="w-4 h-4" />}
              </div>
            </div>
            
            {activeRole === 'speaker' ? (
              <>
                <Button type="button" variant="outline" size="icon" className="border-slate-700 hover:bg-slate-800 text-slate-400">
                  <Mic className="w-4 h-4" />
                </Button>
                <Button type="submit" className="bg-indigo-600 hover:bg-indigo-500">
                  <Send className="w-4 h-4" />
                </Button>
              </>
            ) : (
               <div className="px-3 py-2 text-xs text-green-400 bg-green-500/10 border border-green-500/20 rounded-md flex items-center gap-2">
                 <div className="w-2 h-2 bg-green-500 rounded-full animate-ping" />
                 Listening for signs...
               </div>
            )}
          </form>
        </div>
      </div>
    </div>
  );
}