import React from 'react';
import { motion } from 'framer-motion';
import { ArrowRight, Activity, MessageSquare, Video } from 'lucide-react';
import { Button } from "@/components/ui/button";
import { Link } from 'react-router-dom';
import { createPageUrl } from '@/utils';

export default function Home() {
  return (
    <div className="flex flex-col items-center justify-center min-h-[80vh] text-center">
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5 }}
        className="max-w-3xl space-y-8"
      >
        <div className="inline-flex items-center gap-2 px-4 py-2 rounded-full bg-indigo-500/10 border border-indigo-500/20 text-indigo-400 text-sm font-medium">
          <Activity className="w-4 h-4" />
          <span>Real-time Sign Language Translation</span>
        </div>
        
        <h1 className="text-5xl md:text-7xl font-bold tracking-tight bg-gradient-to-r from-white via-slate-200 to-slate-400 bg-clip-text text-transparent">
          Bridging the gap between <br />
          <span className="text-indigo-400">Sign</span> and <span className="text-purple-400">Speech</span>
        </h1>
        
        <p className="text-xl text-slate-400 max-w-2xl mx-auto leading-relaxed">
          A unified platform facilitating seamless communication through real-time 
          sign language interpretation and text-to-sign synthesis.
        </p>

        <div className="flex flex-col sm:flex-row items-center justify-center gap-4 pt-8">
          <Link to={createPageUrl('Conversation')}>
            <Button size="lg" className="bg-indigo-600 hover:bg-indigo-500 text-white h-14 px-8 text-lg rounded-xl shadow-xl shadow-indigo-500/20 group">
              Start Conversation
              <ArrowRight className="ml-2 w-5 h-5 group-hover:translate-x-1 transition-transform" />
            </Button>
          </Link>
          <Button size="lg" variant="outline" className="h-14 px-8 text-lg rounded-xl border-slate-700 text-slate-300 hover:bg-slate-800 hover:text-white">
            View Documentation
          </Button>
        </div>

        {/* Feature Grid */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6 pt-16 text-left">
          {[
            { icon: Video, title: "Visual Recognition", desc: "Advanced computer vision to detect and interpret hand gestures in real-time." },
            { icon: MessageSquare, title: "Instant Chat", desc: "Seamless text and voice messaging converted instantly to sign language." },
            { icon: Activity, title: "Low Latency", desc: "Powered by UDP sockets for immediate feedback and smooth interaction." }
          ].map((feature, i) => (
            <motion.div
              key={i}
              initial={{ opacity: 0, y: 20 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ delay: 0.2 + (i * 0.1) }}
              className="p-6 rounded-2xl bg-slate-900/50 border border-slate-800 hover:border-indigo-500/30 transition-colors"
            >
              <div className="w-12 h-12 rounded-lg bg-slate-800 flex items-center justify-center mb-4 text-indigo-400">
                <feature.icon className="w-6 h-6" />
              </div>
              <h3 className="text-lg font-semibold text-white mb-2">{feature.title}</h3>
              <p className="text-slate-400 leading-relaxed">{feature.desc}</p>
            </motion.div>
          ))}
        </div>
      </motion.div>
    </div>
  );
}