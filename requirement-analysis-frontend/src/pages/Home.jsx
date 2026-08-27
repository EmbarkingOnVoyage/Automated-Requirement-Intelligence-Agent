// import {
//   FolderOpen,
//   MessageSquare,
//   Video,
//   GitMerge
// } from 'lucide-react';

// const stats = [
//   {
//     title: 'Projects',
//     value: 12,
//     color: 'bg-blue-500',
//   },
//   {
//     title: 'Analyses',
//     value: 48,
//     color: 'bg-green-500',
//   },
//   {
//     title: 'Videos',
//     value: 16,
//     color: 'bg-purple-500',
//   },
//   {
//     title: 'Reports',
//     value: 7,
//     color: 'bg-orange-500',
//   },
// ];

// const actions = [
//   {
//     title: 'Analyze Text',
//     icon: MessageSquare,
//     color: 'bg-blue-500',
//   },
//   {
//     title: 'Analyze Video',
//     icon: Video,
//     color: 'bg-purple-500',
//   },
//   {
//     title: 'Projects',
//     icon: FolderOpen,
//     color: 'bg-green-500',
//   },
//   {
//     title: 'Consolidate',
//     icon: GitMerge,
//     color: 'bg-orange-500',
//   },
// ];

// export default function Home() {
//   return (
//     <div className="p-8">
//       {/* Header */}
//       <div className="mb-8">
//         <h1 className="text-3xl font-bold text-slate-800">
//           Welcome to ARIA
//         </h1>

//         <p className="text-slate-500 mt-2">
//           AI Powered Requirement Analysis Platform
//         </p>
//       </div>

//       {/* Stats */}
//       <div className="grid grid-cols-4 gap-5 mb-8">
//         {stats.map((item) => (
//           <div
//             key={item.title}
//             className="bg-white rounded-2xl p-5 shadow-sm border border-slate-200"
//           >
//             <div
//               className={`w-12 h-12 rounded-xl ${item.color} mb-4`}
//             />

//             <h2 className="text-3xl font-bold text-slate-800">
//               {item.value}
//             </h2>

//             <p className="text-slate-500 text-sm mt-1">
//               {item.title}
//             </p>
//           </div>
//         ))}
//       </div>

//       {/* Quick Actions */}
//       <div>
//         <h2 className="text-xl font-semibold text-slate-700 mb-4">
//           Quick Actions
//         </h2>

//         <div className="grid grid-cols-2 gap-5">
//           {actions.map((action) => {
//             const Icon = action.icon;

//             return (
//               <div
//                 key={action.title}
//                 className="bg-white rounded-2xl p-6 shadow-sm border border-slate-200 hover:shadow-md transition cursor-pointer"
//               >
//                 <div
//                   className={`w-14 h-14 rounded-xl ${action.color} flex items-center justify-center mb-4`}
//                 >
//                   <Icon className="text-white" />
//                 </div>

//                 <h3 className="text-lg font-semibold text-slate-800">
//                   {action.title}
//                 </h3>

//                 <p className="text-slate-500 text-sm mt-1">
//                   Click to continue
//                 </p>
//               </div>
//             );
//           })}
//         </div>
//       </div>
//     </div>
//   );
// }

import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import {
  MessageSquare, Video, FolderOpen,
  GitMerge, Brain, TrendingUp,
  Clock, FileText
} from 'lucide-react';
import { analysisService } from '../services/analysisService';

// ── Stat Card ──────────────────────────────────────────────────────────
function StatCard({ icon: Icon, label, value, color, loading }) {
  return (
    <div className="bg-white rounded-2xl p-5 shadow-sm border border-slate-100 flex items-center gap-4">
      <div className={`w-12 h-12 ${color} rounded-xl flex items-center justify-center flex-shrink-0`}>
        <Icon size={22} className="text-white" />
      </div>
      <div>
        {loading ? (
          <div className="h-7 w-12 bg-slate-100 rounded animate-pulse mb-1" />
        ) : (
          <p className="text-2xl font-bold text-slate-800">{value}</p>
        )}
        <p className="text-slate-500 text-sm">{label}</p>
      </div>
    </div>
  );
}

// ── Quick Action Card ──────────────────────────────────────────────────
function ActionCard({ to, icon: Icon, label, description, color, bgColor }) {
  return (
    <Link
      to={to}
      className="bg-white rounded-2xl p-5 shadow-sm border border-slate-100 hover:shadow-md hover:-translate-y-0.5 transition-all group"
    >
      <div className={`w-12 h-12 ${bgColor} rounded-xl flex items-center justify-center mb-4 group-hover:scale-110 transition-transform`}>
        <Icon size={22} className={color} />
      </div>
      <h3 className="font-semibold text-slate-800 mb-1">{label}</h3>
      <p className="text-slate-400 text-sm">{description}</p>
      <div className="mt-3 flex items-center gap-1 text-xs font-medium text-blue-500">
        Click to continue
        <span className="group-hover:translate-x-1 transition-transform">→</span>
      </div>
    </Link>
  );
}

export default function Home() {
  const [stats, setStats]     = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    analysisService.getStats()
      .then(r => setStats(r.data))
      .catch(() => setStats({
        totalProjects:       0,
        totalAnalyses:       0,
        totalVideoAnalyses:  0,
        totalConsolidations: 0,
        recentActivities:    []
      }))
      .finally(() => setLoading(false));
  }, []);

  const statCards = [
    { icon: FolderOpen,  label: 'Projects',      value: stats?.totalProjects      ?? 0, color: 'bg-blue-500'   },
    { icon: Brain,       label: 'Analyses',       value: stats?.totalAnalyses      ?? 0, color: 'bg-green-500'  },
    { icon: Video,       label: 'Video Analyses', value: stats?.totalVideoAnalyses ?? 0, color: 'bg-purple-500' },
    { icon: FileText,    label: 'Reports',        value: stats?.totalConsolidations?? 0, color: 'bg-orange-500' },
  ];

  const quickActions = [
    {
      to:          '/analyze',
      icon:        MessageSquare,
      label:       'Analyze Text',
      description: 'Paste meeting transcript or chat conversation',
      color:       'text-blue-600',
      bgColor:     'bg-blue-100',
    },
    {
      to:          '/video',
      icon:        Video,
      label:       'Analyze Video',
      description: 'Upload recording — ARIA transcribes and analyzes',
      color:       'text-purple-600',
      bgColor:     'bg-purple-100',
    },
    {
      to:          '/projects',
      icon:        FolderOpen,
      label:       'Projects',
      description: 'Create and manage your analysis projects',
      color:       'text-green-600',
      bgColor:     'bg-green-100',
    },
    // {
    //   to:          '/consolidate',
    //   icon:        GitMerge,
    //   label:       'Consolidate',
    //   description: 'Merge multi-day sessions and detect conflicts',
    //   color:       'text-orange-600',
    //   bgColor:     'bg-orange-100',
    // },
  ];

  return (
    <div className="p-8">

      {/* ── Hero Banner ─────────────────────────────────────────── */}
      <div className="bg-gradient-to-r from-slate-900 via-blue-900 to-slate-900 rounded-2xl p-8 mb-8 relative overflow-hidden">
        {/* Background decoration */}
        <div className="absolute top-0 right-0 w-64 h-64 bg-blue-500 opacity-5 rounded-full -translate-y-1/2 translate-x-1/4" />
        <div className="absolute bottom-0 left-1/2 w-48 h-48 bg-purple-500 opacity-5 rounded-full translate-y-1/2" />

        <div className="relative z-10">
          <div className="flex items-center gap-3 mb-3">
            <div className="w-10 h-10 bg-blue-500 rounded-xl flex items-center justify-center">
              <Brain size={20} className="text-white" />
            </div>
            <div>
              <p className="text-blue-400 text-xs font-semibold uppercase tracking-widest">
                Welcome to
              </p>
              <h1 className="text-white font-bold text-xl leading-tight">ARIA</h1>
            </div>
          </div>

          <h2 className="text-white text-2xl font-bold mb-2">
            AI Powered Requirement Analysis Platform
          </h2>
          <p className="text-blue-200 text-sm max-w-lg">
            Transform conversations, meetings and recordings into structured requirements,
            MOM, action plans and consolidated reports — automatically.
          </p>

          <div className="flex gap-3 mt-5">
            <Link
              to="/analyze"
              className="bg-blue-500 hover:bg-blue-400 text-white text-sm font-medium px-5 py-2.5 rounded-xl transition-colors"
            >
              Start Analyzing →
            </Link>
            <Link
              to="/projects"
              className="bg-white/10 hover:bg-white/20 text-white text-sm font-medium px-5 py-2.5 rounded-xl transition-colors"
            >
              View Projects
            </Link>
          </div>
        </div>
      </div>

      {/* ── Stats Row ───────────────────────────────────────────── */}
      <div className="grid grid-cols-4 gap-4 mb-8">
        {statCards.map((s) => (
          <StatCard
            key={s.label}
            icon={s.icon}
            label={s.label}
            value={s.value}
            color={s.color}
            loading={loading}
          />
        ))}
      </div>

      {/* ── Quick Actions + Recent Activity ─────────────────────── */}
      <div className="grid grid-cols-3 gap-6">

        {/* Quick Actions — 2 cols */}
        <div className="col-span-2">
          <h2 className="text-base font-semibold text-slate-700 mb-4 flex items-center gap-2">
            <TrendingUp size={16} className="text-blue-500" />
            Quick Actions
          </h2>
          <div className="grid grid-cols-2 gap-4">
            {quickActions.map((a) => (
              <ActionCard key={a.to} {...a} />
            ))}
          </div>
        </div>

        {/* Recent Activity — 1 col */}
        <div>
          <h2 className="text-base font-semibold text-slate-700 mb-4 flex items-center gap-2">
            <Clock size={16} className="text-blue-500" />
            Recent Activity
          </h2>

          <div className="bg-white rounded-2xl shadow-sm border border-slate-100 overflow-hidden">
            {loading ? (
              <div className="p-4 space-y-3">
                {[1, 2, 3, 4].map(i => (
                  <div key={i} className="flex gap-3 items-center">
                    <div className="w-8 h-8 bg-slate-100 rounded-lg animate-pulse" />
                    <div className="flex-1">
                      <div className="h-3 bg-slate-100 rounded animate-pulse mb-1.5 w-3/4" />
                      <div className="h-2.5 bg-slate-100 rounded animate-pulse w-1/2" />
                    </div>
                  </div>
                ))}
              </div>
            ) : stats?.recentActivities?.length > 0 ? (
              <div className="divide-y divide-slate-50">
                {stats.recentActivities.map((a, i) => (
                  <div key={i} className="flex items-center gap-3 px-4 py-3 hover:bg-slate-50 transition-colors">
                    <div className={`w-8 h-8 rounded-lg flex items-center justify-center flex-shrink-0 ${
                      a.sourceType === 'video' ? 'bg-purple-100' : 'bg-blue-100'
                    }`}>
                      {a.sourceType === 'video'
                        ? <Video size={14} className="text-purple-600" />
                        : <MessageSquare size={14} className="text-blue-600" />
                      }
                    </div>
                    <div className="flex-1 min-w-0">
                      <p className="text-sm font-medium text-slate-700 truncate">
                        {a.projectTitle || a.projectName || 'Untitled'}
                      </p>
                      <p className="text-xs text-slate-400">
                        {a.projectName} •{' '}
                        {new Date(a.createdAt).toLocaleDateString('en-GB', {
                          day: '2-digit', month: 'short'
                        })}
                      </p>
                    </div>
                    <span className={`text-xs px-2 py-0.5 rounded-full font-medium flex-shrink-0 ${
                      a.sourceType === 'video'
                        ? 'bg-purple-100 text-purple-600'
                        : 'bg-blue-100 text-blue-600'
                    }`}>
                      {a.sourceType || 'text'}
                    </span>
                  </div>
                ))}
              </div>
            ) : (
              <div className="py-10 text-center">
                <Brain size={28} className="mx-auto text-slate-200 mb-2" />
                <p className="text-slate-400 text-sm">No activity yet</p>
                <p className="text-slate-300 text-xs mt-1">
                  Start by analyzing a conversation
                </p>
              </div>
            )}

            {stats?.recentActivities?.length > 0 && (
              <div className="px-4 py-2.5 border-t border-slate-50">
                <Link
                  to="/history"
                  className="text-xs text-blue-500 hover:underline font-medium"
                >
                  View all history →
                </Link>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}