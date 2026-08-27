
// import { useState, useEffect } from 'react';
// import { analysisService } from '../services/analysisService';
// import { projectService } from '../services/projectService';
// import toast from 'react-hot-toast';
// import Loader from '../components/UI/Loader';
// import { GitMerge, AlertTriangle, CheckCircle, Clock, ChevronRight } from 'lucide-react';

// // ── Tab Button ─────────────────────────────────────────────────────────
// function TabButton({ active, onClick, icon, label, count }) {
//   return (
//     <button
//       onClick={onClick}
//       className={`flex items-center gap-2 px-5 py-3 text-sm font-medium border-b-2 transition-all ${
//         active
//           ? 'border-purple-600 text-purple-600'
//           : 'border-transparent text-slate-500 hover:text-slate-700'
//       }`}
//     >
//       {icon}
//       {label}
//       {count !== undefined && (
//         <span className={`text-xs px-2 py-0.5 rounded-full font-semibold ${
//           active ? 'bg-purple-100 text-purple-600' : 'bg-slate-100 text-slate-500'
//         }`}>
//           {count}
//         </span>
//       )}
//     </button>
//   );
// }

// // ── Section List ───────────────────────────────────────────────────────
// function SectionList({ title, items = [], icon, borderColor = 'border-blue-400', bgColor = 'bg-blue-50' }) {
//   if (!items || items.length === 0) return null;
//   return (
//     <div className="mb-5">
//       <h4 className="text-sm font-semibold text-slate-700 mb-2 flex items-center gap-2">
//         {icon} {title}
//         <span className="text-xs bg-slate-100 text-slate-500 px-2 py-0.5 rounded-full">
//           {items.length}
//         </span>
//       </h4>
//       <ul className="space-y-1.5">
//         {items.map((item, i) => (
//           <li key={i} className={`text-sm text-slate-600 ${bgColor} rounded-lg px-3 py-2 border-l-2 ${borderColor}`}>
//             {item}
//           </li>
//         ))}
//       </ul>
//     </div>
//   );
// }

// export default function ConsolidatedReport() {
//   const [projects, setProjects]   = useState([]);
//   const [selected, setSelected]   = useState('');
//   const [loading, setLoading]     = useState(false);
//   const [report, setReport]       = useState(null);
//   const [activeTab, setActiveTab] = useState('overview');
//   const [history, setHistory]     = useState([]);

//   useEffect(() => {
//     projectService.getAll()
//       .then(r => setProjects(r.data))
//       .catch(() => {});
//   }, []);

//   // Load history when project selected
//   const handleProjectChange = async (projectId) => {
//     setSelected(projectId);
//     setReport(null);
//     if (!projectId) return;

//     try {
//       const res = await analysisService.getHistory(projectId);
//       setHistory(res.data);
//     } catch {
//       setHistory([]);
//     }
//   };

//   const handleConsolidate = async () => {
//     if (!selected) return toast.error('Please select a project');
//     if (history.length === 0)
//       return toast.error('No analyses found for this project. Analyze some conversations first.');
//     if (history.length < 2)
//       return toast.error('Need at least 2 analyses to consolidate.');

//     setLoading(true);
//     setReport(null);

//     try {
//       const res = await analysisService.consolidate(selected);
//       setReport(res.data);
//       setActiveTab('overview');
//       toast.success('Consolidation complete!');
//     } catch (err) {
//       toast.error(err.message);
//     } finally {
//       setLoading(false);
//     }
//   };

//   return (
//     <div className="p-8">
//       {/* Header */}
//       <div className="mb-6">
//         <h1 className="text-2xl font-bold text-slate-800 flex items-center gap-2">
//           <GitMerge size={24} className="text-purple-600" />
//           Consolidate Project
//         </h1>
//         <p className="text-slate-500 text-sm mt-1">
//           Merge all session analyses — detect changes, resolve conflicts, produce final requirements
//         </p>
//       </div>

//       {/* Project Selector Card */}
//       <div className="bg-white rounded-xl shadow-sm border border-slate-100 p-5 mb-6">
//         <div className="flex items-end gap-4">
//           <div className="flex-1">
//             <label className="text-sm font-medium text-slate-600">Select Project</label>
//             <select
//               className="w-full mt-1 px-3 py-2 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-purple-500"
//               value={selected}
//               onChange={e => handleProjectChange(e.target.value)}
//             >
//               <option value="">Choose a project...</option>
//               {projects.map(p => (
//                 <option key={p.id} value={p.id}>{p.name}</option>
//               ))}
//             </select>
//           </div>

//           {/* Session count */}
//           {selected && (
//             <div className="flex items-center gap-3">
//               <div className="text-center bg-purple-50 rounded-lg px-4 py-2">
//                 <p className="text-2xl font-bold text-purple-600">{history.length}</p>
//                 <p className="text-xs text-slate-500">Sessions found</p>
//               </div>
//             </div>
//           )}

//           <button
//             onClick={handleConsolidate}
//             disabled={loading || !selected}
//             className="bg-purple-600 text-white px-6 py-2.5 rounded-lg font-medium hover:bg-purple-700 disabled:opacity-50 transition-colors flex items-center gap-2"
//           >
//             {loading ? (
//               <>
//                 <div className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin" />
//                 Consolidating...
//               </>
//             ) : (
//               <>
//                 <GitMerge size={16} />
//                 Consolidate All
//               </>
//             )}
//           </button>
//         </div>

//         {/* Session list preview */}
//         {history.length > 0 && (
//           <div className="mt-4 pt-4 border-t border-slate-50">
//             <p className="text-xs font-medium text-slate-500 mb-2">Sessions to consolidate:</p>
//             <div className="flex flex-wrap gap-2">
//               {history.map((h, i) => (
//                 <div key={i} className="flex items-center gap-1.5 bg-slate-50 rounded-lg px-3 py-1.5">
//                   <div className="w-5 h-5 bg-purple-100 rounded-full flex items-center justify-center">
//                     <span className="text-xs font-bold text-purple-600">{i + 1}</span>
//                   </div>
//                   <span className="text-xs text-slate-600 max-w-32 truncate">{h.title}</span>
//                   <span className="text-xs text-slate-400">
//                     {new Date(h.createdAt).toLocaleDateString('en-GB', {
//                       day: '2-digit', month: 'short'
//                     })}
//                   </span>
//                 </div>
//               ))}
//             </div>
//           </div>
//         )}
//       </div>

//       {/* Loading */}
//       {loading && (
//         <div className="bg-white rounded-xl shadow-sm border border-slate-100 p-10">
//           <Loader text="ARIA is analyzing all sessions and resolving conflicts..." />
//           <div className="mt-6 max-w-sm mx-auto space-y-2">
//             {[
//               'Reading all session transcripts...',
//               'Detecting requirement changes...',
//               'Identifying conflicts...',
//               'Resolving conflicts using latest session...',
//               'Generating final consolidated requirements...',
//             ].map((step, i) => (
//               <div key={i} className="flex items-center gap-2 text-xs text-slate-400">
//                 <div className="w-1.5 h-1.5 bg-purple-400 rounded-full animate-pulse" />
//                 {step}
//               </div>
//             ))}
//           </div>
//         </div>
//       )}

//       {/* Report */}
//       {report && !loading && (
//         <div className="bg-white rounded-xl shadow-sm border border-slate-100 overflow-hidden">

//           {/* Report Header */}
//           <div className="p-5 border-b border-slate-100 bg-gradient-to-r from-purple-50 to-white">
//             <h2 className="text-lg font-bold text-slate-800">
//               {report.projectTitle || 'Consolidated Report'}
//             </h2>
//             {report.projectObjective && (
//               <p className="text-slate-500 text-sm mt-1">{report.projectObjective}</p>
//             )}
//             <div className="flex gap-3 mt-3">
//               <span className="bg-purple-100 text-purple-700 text-xs px-3 py-1 rounded-full font-medium">
//                 {report.totalConversationsAnalyzed} sessions analyzed
//               </span>
//               <span className="bg-orange-100 text-orange-700 text-xs px-3 py-1 rounded-full font-medium">
//                 {report.detectedChanges?.length || 0} changes detected
//               </span>
//               <span className="bg-red-100 text-red-700 text-xs px-3 py-1 rounded-full font-medium">
//                 {report.detectedConflicts?.length || 0} conflicts resolved
//               </span>
//               <span className="bg-green-100 text-green-700 text-xs px-3 py-1 rounded-full font-medium">
//                 {report.finalFunctionalRequirements?.length || 0} final requirements
//               </span>
//             </div>
//           </div>

//           {/* Tabs */}
//           <div className="border-b border-slate-100 flex overflow-x-auto">
//             <TabButton
//               active={activeTab === 'overview'}
//               onClick={() => setActiveTab('overview')}
//               icon={<GitMerge size={15} />}
//               label="Overview"
//             />
//             <TabButton
//               active={activeTab === 'changes'}
//               onClick={() => setActiveTab('changes')}
//               icon={<Clock size={15} />}
//               label="Changes"
//               count={report.detectedChanges?.length || 0}
//             />
//             <TabButton
//               active={activeTab === 'conflicts'}
//               onClick={() => setActiveTab('conflicts')}
//               icon={<AlertTriangle size={15} />}
//               label="Conflicts"
//               count={report.detectedConflicts?.length || 0}
//             />
//             <TabButton
//               active={activeTab === 'requirements'}
//               onClick={() => setActiveTab('requirements')}
//               icon={<CheckCircle size={15} />}
//               label="Final Requirements"
//               count={report.finalFunctionalRequirements?.length || 0}
//             />
//             <TabButton
//               active={activeTab === 'evolved'}
//               onClick={() => setActiveTab('evolved')}
//               icon={<ChevronRight size={15} />}
//               label="Evolution"
//               count={(report.evolvedRequirements?.length || 0) + (report.newlyAddedRequirements?.length || 0)}
//             />
//           </div>

//           {/* Tab Content */}
//           <div className="p-6 max-h-[60vh] overflow-y-auto">

//             {/* ── Overview Tab ──────────────────────────── */}
//             {activeTab === 'overview' && (
//               <div className="space-y-5">
//                 {/* Summary Cards */}
//                 <div className="grid grid-cols-4 gap-3">
//                   {[
//                     { label: 'Sessions',        value: report.totalConversationsAnalyzed, color: 'purple' },
//                     { label: 'Changes',         value: report.detectedChanges?.length || 0, color: 'orange' },
//                     { label: 'Conflicts',       value: report.detectedConflicts?.length || 0, color: 'red' },
//                     { label: 'Final FRs',       value: report.finalFunctionalRequirements?.length || 0, color: 'green' },
//                   ].map((stat, i) => (
//                     <div key={i} className={`bg-${stat.color}-50 rounded-xl p-4 text-center`}>
//                       <p className={`text-2xl font-bold text-${stat.color}-600`}>{stat.value}</p>
//                       <p className="text-xs text-slate-500 mt-1">{stat.label}</p>
//                     </div>
//                   ))}
//                 </div>

//                 <SectionList
//                   title="Final Business Rules"
//                   items={report.finalBusinessRules}
//                   icon="📋"
//                   borderColor="border-green-400"
//                   bgColor="bg-green-50"
//                 />
//                 <SectionList
//                   title="Final Milestones"
//                   items={report.finalMilestones}
//                   icon="📅"
//                   borderColor="border-purple-400"
//                   bgColor="bg-purple-50"
//                 />
//                 <SectionList
//                   title="Open Questions"
//                   items={report.finalOpenQuestions}
//                   icon="❓"
//                   borderColor="border-orange-400"
//                   bgColor="bg-orange-50"
//                 />
//                 <SectionList
//                   title="Risk Flags"
//                   items={report.finalRiskFlags}
//                   icon="🚩"
//                   borderColor="border-red-400"
//                   bgColor="bg-red-50"
//                 />

//                 {/* Prioritization */}
//                 {report.finalPrioritization && (
//                   <div>
//                     <h4 className="text-sm font-semibold text-slate-700 mb-3">🎯 Final Prioritization</h4>
//                     <div className="grid grid-cols-3 gap-3">
//                       {[
//                         { label: 'Must Have',   items: report.finalPrioritization.mustHave,   color: 'red'    },
//                         { label: 'Should Have', items: report.finalPrioritization.shouldHave, color: 'orange' },
//                         { label: 'Nice to Have',items: report.finalPrioritization.niceToHave, color: 'green'  },
//                       ].map((p, i) => (
//                         <div key={i} className={`bg-${p.color}-50 rounded-xl p-3`}>
//                           <p className={`text-xs font-semibold text-${p.color}-600 mb-2`}>{p.label}</p>
//                           <ul className="space-y-1">
//                             {(p.items || []).map((item, j) => (
//                               <li key={j} className="text-xs text-slate-600">• {item}</li>
//                             ))}
//                           </ul>
//                         </div>
//                       ))}
//                     </div>
//                   </div>
//                 )}
//               </div>
//             )}

//             {/* ── Changes Tab ───────────────────────────── */}
//             {activeTab === 'changes' && (
//               <div>
//                 {!report.detectedChanges?.length ? (
//                   <div className="text-center py-12 text-slate-400">
//                     <Clock size={40} className="mx-auto mb-3 opacity-30" />
//                     <p>No changes detected across sessions</p>
//                   </div>
//                 ) : (
//                   <div className="space-y-3">
//                     {report.detectedChanges.map((c, i) => (
//                       <div key={i} className={`rounded-xl p-4 border ${
//                         c.type === 'ADDED'    ? 'bg-green-50 border-green-100' :
//                         c.type === 'REMOVED'  ? 'bg-red-50 border-red-100'    :
//                         'bg-orange-50 border-orange-100'
//                       }`}>
//                         <div className="flex items-center gap-2 mb-2">
//                           <span className={`text-xs font-bold px-2 py-0.5 rounded-full ${
//                             c.type === 'ADDED'    ? 'bg-green-200 text-green-700'   :
//                             c.type === 'REMOVED'  ? 'bg-red-200 text-red-700'       :
//                             'bg-orange-200 text-orange-700'
//                           }`}>
//                             {c.type}
//                           </span>
//                           <span className="text-xs text-slate-500 font-medium">{c.day}</span>
//                         </div>
//                         {c.oldValue && (
//                           <p className="text-xs text-slate-400 line-through mb-1">
//                             Before: {c.oldValue}
//                           </p>
//                         )}
//                         {c.newValue && (
//                           <p className="text-sm text-slate-700 mb-1">
//                             After: {c.newValue}
//                           </p>
//                         )}
//                         {c.reason && (
//                           <p className="text-xs text-slate-400 italic mt-1">
//                             💡 {c.reason}
//                           </p>
//                         )}
//                       </div>
//                     ))}
//                   </div>
//                 )}
//               </div>
//             )}

//             {/* ── Conflicts Tab ─────────────────────────── */}
//             {activeTab === 'conflicts' && (
//               <div>
//                 {!report.detectedConflicts?.length ? (
//                   <div className="text-center py-12 text-slate-400">
//                     <CheckCircle size={40} className="mx-auto mb-3 opacity-30" />
//                     <p className="font-medium text-green-600">No conflicts detected!</p>
//                     <p className="text-sm mt-1">All sessions are consistent</p>
//                   </div>
//                 ) : (
//                   <div className="space-y-3">
//                     {report.detectedConflicts.map((c, i) => (
//                       <div key={i} className="bg-red-50 border border-red-100 rounded-xl p-4">
//                         <div className="flex items-start gap-2">
//                           <AlertTriangle size={16} className="text-red-500 mt-0.5 flex-shrink-0" />
//                           <div className="flex-1">
//                             <p className="text-sm font-medium text-red-700">
//                               {c.conflictDescription}
//                             </p>
//                             <p className="text-xs text-slate-400 mt-1">{c.fromDay}</p>
//                             {c.resolution && (
//                               <div className="mt-2 flex items-start gap-1.5">
//                                 <CheckCircle size={13} className="text-green-500 mt-0.5" />
//                                 <p className="text-xs text-green-700">{c.resolution}</p>
//                               </div>
//                             )}
//                           </div>
//                         </div>
//                       </div>
//                     ))}
//                   </div>
//                 )}
//               </div>
//             )}

//             {/* ── Final Requirements Tab ────────────────── */}
//             {activeTab === 'requirements' && (
//               <div className="space-y-5">
//                 <SectionList
//                   title="Functional Requirements"
//                   items={report.finalFunctionalRequirements}
//                   icon="⚡"
//                   borderColor="border-blue-400"
//                   bgColor="bg-blue-50"
//                 />
//                 <SectionList
//                   title="Non-Functional Requirements"
//                   items={report.finalNonFunctionalRequirements}
//                   icon="🔒"
//                   borderColor="border-gray-400"
//                   bgColor="bg-gray-50"
//                 />
//                 <SectionList
//                   title="User Stories"
//                   items={report.finalUserStories}
//                   icon="👤"
//                   borderColor="border-purple-400"
//                   bgColor="bg-purple-50"
//                 />
//                 <SectionList
//                   title="Modules"
//                   items={report.finalModules}
//                   icon="🧩"
//                   borderColor="border-blue-400"
//                   bgColor="bg-blue-50"
//                 />
//                 <SectionList
//                   title="API Suggestions"
//                   items={report.finalApiSuggestions}
//                   icon="🔗"
//                   borderColor="border-purple-400"
//                   bgColor="bg-purple-50"
//                 />
//                 <SectionList
//                   title="Database Entities"
//                   items={report.finalDatabaseEntities}
//                   icon="🗄️"
//                   borderColor="border-orange-400"
//                   bgColor="bg-orange-50"
//                 />
//                 <SectionList
//                   title="Roles"
//                   items={report.finalRoles}
//                   icon="👥"
//                   borderColor="border-gray-400"
//                   bgColor="bg-gray-50"
//                 />
//                 <SectionList
//                   title="Assumptions"
//                   items={report.finalAssumptions}
//                   icon="💭"
//                   borderColor="border-blue-300"
//                   bgColor="bg-blue-50"
//                 />
//               </div>
//             )}

//             {/* ── Evolution Tab ─────────────────────────── */}
//             {activeTab === 'evolved' && (
//               <div className="space-y-5">
//                 <SectionList
//                   title="Newly Added Requirements"
//                   items={report.newlyAddedRequirements}
//                   icon="✅"
//                   borderColor="border-green-400"
//                   bgColor="bg-green-50"
//                 />
//                 <SectionList
//                   title="Evolved Requirements"
//                   items={report.evolvedRequirements}
//                   icon="🔄"
//                   borderColor="border-blue-400"
//                   bgColor="bg-blue-50"
//                 />
//                 <SectionList
//                   title="Dropped Requirements"
//                   items={report.droppedRequirements}
//                   icon="🗑️"
//                   borderColor="border-red-400"
//                   bgColor="bg-red-50"
//                 />
//               </div>
//             )}

//           </div>
//         </div>
//       )}

//       {/* Empty state — no project selected */}
//       {!selected && !loading && !report && (
//         <div className="bg-white rounded-xl shadow-sm border border-slate-100 p-12 text-center">
//           <div className="w-16 h-16 bg-purple-100 rounded-2xl flex items-center justify-center mx-auto mb-4">
//             <GitMerge size={28} className="text-purple-500" />
//           </div>
//           <p className="font-semibold text-slate-700">Select a project to consolidate</p>
//           <p className="text-slate-400 text-sm mt-1 max-w-sm mx-auto">
//             ARIA will read all session analyses for the selected project,
//             detect changes, resolve conflicts and produce one final requirement document
//           </p>
//         </div>
//       )}
//     </div>
//   );
// }
