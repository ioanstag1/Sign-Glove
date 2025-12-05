import React from 'react';
import { motion } from 'framer-motion';
import { User, Video, Mic, Settings, MoreVertical, Activity } from 'lucide-react';
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";

export default function Users() {
  const users = [
    {
      id: 1,
      name: "Speaker User",
      role: "Speaker (Hearing)",
      status: "Online",
      device: "Microphone & Keyboard",
      activity: "Typing...",
      avatarColor: "bg-indigo-500",
      icon: Mic
    },
    {
      id: 2,
      name: "Signer User",
      role: "Signer (Deaf/HoH)",
      status: "Online",
      device: "Webcam & Motion Tracking",
      activity: "Tracking Hand Gestures",
      avatarColor: "bg-green-500",
      icon: Video
    }
  ];

  return (
    <div className="max-w-4xl mx-auto">
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-white">Session Participants</h1>
        <p className="text-slate-400 mt-2">Manage active users in the current translation session.</p>
      </div>

      <div className="grid gap-6 md:grid-cols-2">
        {users.map((user, index) => (
          <motion.div
            key={user.id}
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: index * 0.1 }}
          >
            <Card className="bg-slate-900 border-slate-800 overflow-hidden hover:border-indigo-500/30 transition-colors">
              <CardHeader className="flex flex-row items-start justify-between pb-2">
                <div className="flex gap-4">
                  <Avatar className="h-12 w-12 border-2 border-slate-800">
                    <AvatarFallback className={`${user.avatarColor} text-white`}>
                      {user.name.charAt(0)}
                    </AvatarFallback>
                  </Avatar>
                  <div>
                    <CardTitle className="text-lg text-white">{user.name}</CardTitle>
                    <div className="flex items-center gap-2 mt-1">
                      <Badge variant="secondary" className="bg-slate-800 text-slate-300 border-slate-700">
                        {user.role}
                      </Badge>
                      <span className="flex items-center text-xs text-green-400">
                        <span className="w-1.5 h-1.5 rounded-full bg-green-500 mr-1.5 animate-pulse" />
                        {user.status}
                      </span>
                    </div>
                  </div>
                </div>
                <Button variant="ghost" size="icon" className="text-slate-400">
                  <MoreVertical className="w-4 h-4" />
                </Button>
              </CardHeader>
              <CardContent className="mt-4 space-y-4">
                <div className="p-3 bg-slate-950/50 rounded-lg border border-slate-800 space-y-3">
                  <div className="flex items-center justify-between text-sm">
                    <span className="text-slate-400 flex items-center gap-2">
                      <Settings className="w-3 h-3" /> Input Device
                    </span>
                    <span className="text-slate-200">{user.device}</span>
                  </div>
                  <div className="flex items-center justify-between text-sm">
                    <span className="text-slate-400 flex items-center gap-2">
                      <Activity className="w-3 h-3" /> Current Activity
                    </span>
                    <span className="text-slate-200">{user.activity}</span>
                  </div>
                </div>
                
                <div className="flex gap-2">
                  <Button variant="outline" className="w-full border-slate-700 hover:bg-slate-800 text-slate-300">
                    Configure
                  </Button>
                  <Button variant="outline" className="w-full border-slate-700 hover:bg-slate-800 text-slate-300">
                    View Stats
                  </Button>
                </div>
              </CardContent>
            </Card>
          </motion.div>
        ))}
      </div>
    </div>
  );
}