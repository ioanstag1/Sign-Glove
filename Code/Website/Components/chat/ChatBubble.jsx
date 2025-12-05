import React, { useEffect, useRef } from 'react';
import { motion } from 'framer-motion';
import { Avatar } from "@/components/ui/avatar";
import { cn } from "@/lib/utils";

export default function ChatBubble({ message, isMe }) {
  return (
    <motion.div
      initial={{ opacity: 0, y: 10, scale: 0.95 }}
      animate={{ opacity: 1, y: 0, scale: 1 }}
      className={cn(
        "flex gap-3 max-w-[80%]",
        isMe ? "ml-auto flex-row-reverse" : "mr-auto"
      )}
    >
      <div className={cn(
        "w-8 h-8 rounded-full flex items-center justify-center text-xs font-bold flex-shrink-0",
        isMe ? "bg-indigo-500 text-white" : "bg-purple-500 text-white"
      )}>
        {isMe ? "ME" : "OT"}
      </div>
      
      <div className={cn(
        "p-4 rounded-2xl shadow-sm text-sm leading-relaxed",
        isMe 
          ? "bg-indigo-600 text-white rounded-tr-sm" 
          : "bg-slate-800 text-slate-200 rounded-tl-sm border border-slate-700"
      )}>
        <p>{message.content}</p>
        <span className={cn(
          "text-[10px] mt-2 block opacity-60",
          isMe ? "text-indigo-200" : "text-slate-500"
        )}>
          {new Date(message.timestamp || Date.now()).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
        </span>
      </div>
    </motion.div>
  );
}