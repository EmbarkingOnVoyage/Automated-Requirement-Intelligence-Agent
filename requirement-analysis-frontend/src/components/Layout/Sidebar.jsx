import { NavLink } from 'react-router-dom';

import {
  LayoutDashboard,
  FolderOpen,
  MessageSquare,
  Video,
  History,
  GitMerge
} from 'lucide-react';

const navItems = [
  {
    to: '/',
    label: 'Dashboard',
    icon: LayoutDashboard,
  },
  {
    to: '/projects',
    label: 'Projects',
    icon: FolderOpen,
  },
  {
    to: '/analyze',
    label: 'Analyze',
    icon: MessageSquare,
  },
  {
    to: '/video',
    label: 'Video Analyze',
    icon: Video,
  },
  // {
  //   to: '/consolidate',
  //   label: 'Consolidate',
  //   icon: GitMerge,
  // },
  {
    to: '/history',
    label: 'History',
    icon: History,
  },
];

export default function Sidebar() {
  return (
    <aside className="w-64 min-h-screen bg-slate-900 text-white flex flex-col">

      {/* Logo */}
      <div className="p-6 border-b border-slate-700">
        <h1 className="text-2xl font-bold text-blue-400">
          ARIA
        </h1>

        <p className="text-sm text-slate-400 mt-1">
          Requirement Analysis Agent
        </p>
      </div>

      {/* Navigation */}
      <nav className="flex-1 p-4 space-y-2">

        {navItems.map((item) => {
          const Icon = item.icon;

          return (
            <NavLink
              key={item.to}
              to={item.to}
              end={item.to === '/'}
              className={({ isActive }) =>
                `flex items-center gap-3 px-4 py-3 rounded-xl transition-all text-sm font-medium ${
                  isActive
                    ? 'bg-blue-600 text-white'
                    : 'text-slate-300 hover:bg-slate-800 hover:text-white'
                }`
              }
            >
              <Icon size={18} />

              <span>
                {item.label}
              </span>
            </NavLink>
          );
        })}

      </nav>

      {/* Footer */}
      <div className="p-4 border-t border-slate-700">
        <p className="text-xs text-slate-500">
          v1.0.0 Powered by AI
        </p>
      </div>

    </aside>
  );
}