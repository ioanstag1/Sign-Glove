import React from 'react';
import { Link, useLocation } from 'react-router-dom';
import { createPageUrl } from '@/utils';
import { MessageSquare, Users, Settings, Home, Github, Bell, Share2 } from 'lucide-react';
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";

export default function Layout({ children }) {
  const location = useLocation();
  const navItems = [
    { icon: Home, label: 'Home', path: 'Home' },
    { icon: MessageSquare, label: 'Conversation', path: 'Conversation' },
    { icon: Users, label: 'Users', path: 'Users' },
  ];

  return (
    <div className="min-h-screen bg-slate-950 text-slate-100 font-sans selection:bg-indigo-500/30">
      {/* Top Navigation */}
      <header className="sticky top-0 z-50 border-b border-slate-800 bg-slate-950/80 backdrop-blur-xl">
        <div className="max-w-7xl mx-auto px-4 h-16 flex items-center justify-between">
          <div className="flex items-center gap-2">
            <div className="w-8 h-8 bg-gradient-to-br from-indigo-500 to-purple-600 rounded-lg flex items-center justify-center shadow-lg shadow-indigo-500/20">
              <span className="font-bold text-white">S</span>
            </div>
            <span className="font-semibold text-lg tracking-tight">SignSync</span>
          </div>

          <nav className="hidden md:flex items-center gap-1">
            {navItems.map((item) => (
              <Link key={item.path} to={createPageUrl(item.path)}>
                <Button
                  variant="ghost"
                  className={`gap-2 text-sm font-medium transition-all duration-200 ${
                    location.pathname.includes(item.path)
                      ? 'bg-slate-800/50 text-indigo-400'
                      : 'text-slate-400 hover:text-slate-100 hover:bg-slate-800/30'
                  }`}
                >
                  <item.icon className="w-4 h-4" />
                  {item.label}
                </Button>
              </Link>
            ))}
          </nav>

          <div className="flex items-center gap-2">
            <Dialog>
              <DialogTrigger asChild>
                <Button variant="outline" size="sm" className="hidden sm:flex gap-2 border-slate-700 bg-slate-900 text-slate-300 hover:bg-indigo-600 hover:text-white hover:border-indigo-500 transition-all">
                  <Share2 className="w-4 h-4" />
                  Share App
                </Button>
              </DialogTrigger>
              <DialogContent className="bg-slate-900 border-slate-800 text-white">
                <DialogHeader>
                  <DialogTitle>Share Application</DialogTitle>
                  <DialogDescription className="text-slate-400">
                    Your app is live! Share this URL with others so they can join the conversation.
                  </DialogDescription>
                </DialogHeader>
                <div className="flex gap-2 mt-2">
                  <Input value={window.location.origin} readOnly className="bg-slate-950 border-slate-800 text-slate-300" />
                  <Button onClick={() => navigator.clipboard.writeText(window.location.origin)}>Copy</Button>
                </div>
                <div className="mt-4 p-4 bg-slate-950 rounded-lg border border-slate-800 text-sm text-slate-400">
                  <p><strong>Status:</strong> 🟢 Live & Hosted on Base44</p>
                  <p className="mt-1">You don't need to run any servers. This URL is permanent.</p>
                </div>
              </DialogContent>
            </Dialog>

            <Button variant="ghost" size="icon" className="text-slate-400 hover:text-slate-100">
              <Bell className="w-5 h-5" />
            </Button>
            <div className="w-8 h-8 rounded-full bg-slate-800 border border-slate-700 flex items-center justify-center text-sm font-medium">
              JD
            </div>
          </div>
        </div>
      </header>

      <main className="max-w-7xl mx-auto px-4 py-6">
        {children}
      </main>
    </div>
  );
}