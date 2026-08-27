// // export default function VideoAnalyze() {
// //   return (
// //     <div className="p-8">
// //       <h1 className="text-3xl font-bold">
// //         Video Analyze Page
// //       </h1>
// //     </div>
// //   );
// // }

// import { useState, useEffect } from 'react';
// import { analysisService } from '../services/analysisService';
// import { projectService } from '../services/projectService';
// import toast from 'react-hot-toast';
// import Loader from '../components/UI/Loader';
// import AnalysisResult from './AnalysisResult';
// import { Video, Link, FolderOpen, ChevronDown, ChevronUp } from 'lucide-react';

// export default function VideoAnalyze() {
//   const [projects, setProjects]         = useState([]);
//   const [loading, setLoading]           = useState(false);
//   const [result, setResult]             = useState(null);
//   const [showTranscript, setShowTranscript] = useState(true);
//   const [inputMethod, setInputMethod]   = useState('url'); // 'url' | 'local'
//   const [form, setForm] = useState({
//     projectId:        '',
//     newProjectName:   '',
//     newProjectDomain: '',
//     videoUrl:         '',
//     videoFilePath:    '',
//     useNewProject:    false,
//   });

//   useEffect(() => {
//     projectService.getAll()
//       .then(r => setProjects(r.data))
//       .catch(() => {});
//   }, []);

//   const handleSubmit = async (e) => {
//     e.preventDefault();

//     // Validation
//     if (!form.useNewProject && !form.projectId)
//       return toast.error('Please select a project');
//     if (form.useNewProject && !form.newProjectName.trim())
//       return toast.error('Please enter a project name');
//     if (inputMethod === 'url' && !form.videoUrl.trim())
//       return toast.error('Please enter a video URL');
//     if (inputMethod === 'local' && !form.videoFilePath.trim())
//       return toast.error('Please enter a file path');

//     setLoading(true);
//     setResult(null);

//     try {
//       const base = form.useNewProject
//         ? { newProjectName: form.newProjectName, newProjectDomain: form.newProjectDomain }
//         : { projectId: parseInt(form.projectId) };

//       const payload = {
//         ...base,
//         videoUrl:      inputMethod === 'url'   ? form.videoUrl      : null,
//         videoFilePath: inputMethod === 'local' ? form.videoFilePath  : null,
//       };

//       const res = await analysisService.analyzeVideo(payload);
//       setResult(res.data);
//       toast.success('Analysis complete!');
//     } catch (err) {
//       toast.error(err.message);
//     } finally {
//       setLoading(false);
//     }
//   };

//   return (
//     <div className="p-8">
//       {/* Page Header */}
//       <div className="mb-6">
//         <h1 className="text-2xl font-bold text-slate-800 flex items-center gap-2">
//           <Video size={24} className="text-purple-600" />
//           Video Analysis
//         </h1>
//         <p className="text-slate-500 text-sm mt-1">
//           Upload a meeting recording — ARIA will transcribe and analyze it automatically
//         </p>
//       </div>

//       <div className="grid grid-cols-5 gap-6">

//         {/* ── Left — Input Form (2 cols) ───────────────────── */}
//         <div className="col-span-2">
//           <div className="bg-white rounded-xl shadow-sm border border-slate-100 p-6 h-[380px]">

//             {/* Input Method Toggle */}
//             <div className="mb-5">
//               <p className="text-sm font-medium text-slate-600 mb-2">Input Method</p>
//               <div className="grid grid-cols-2 gap-2">
//                 <button
//                   type="button"
//                   onClick={() => setInputMethod('url')}
//                   className={`flex items-center justify-center gap-2 py-2.5 rounded-lg text-sm font-medium transition-all ${
//                     inputMethod === 'url'
//                       ? 'bg-purple-600 text-white shadow-sm'
//                       : 'bg-slate-100 text-slate-600 hover:bg-slate-200'
//                   }`}
//                 >
//                   <Link size={15} /> Video URL
//                 </button>
//                 <button
//                   type="button"
//                   onClick={() => setInputMethod('local')}
//                   className={`flex items-center justify-center gap-2 py-2.5 rounded-lg text-sm font-medium transition-all ${
//                     inputMethod === 'local'
//                       ? 'bg-purple-600 text-white shadow-sm'
//                       : 'bg-slate-100 text-slate-600 hover:bg-slate-200'
//                   }`}
//                 >
//                   <FolderOpen size={15} /> Local File
//                 </button>
//               </div>
//             </div>

//             <form onSubmit={handleSubmit} className="space-y-4">

//               {/* Project Selection */}
//               <div>
//                 <p className="text-sm font-medium text-slate-600 mb-2">Project</p>
//                 <div className="flex gap-2 mb-2">
//                   <button
//                     type="button"
//                     onClick={() => setForm({ ...form, useNewProject: false })}
//                     className={`text-xs px-3 py-1.5 rounded-lg font-medium transition-colors ${
//                       !form.useNewProject
//                         ? 'bg-blue-600 text-white'
//                         : 'bg-slate-100 text-slate-600'
//                     }`}
//                   >
//                     Existing
//                   </button>
//                   <button
//                     type="button"
//                     onClick={() => setForm({ ...form, useNewProject: true })}
//                     className={`text-xs px-3 py-1.5 rounded-lg font-medium transition-colors ${
//                       form.useNewProject
//                         ? 'bg-blue-600 text-white'
//                         : 'bg-slate-100 text-slate-600'
//                     }`}
//                   >
//                     + New Project
//                   </button>
//                 </div>

//                 {!form.useNewProject ? (
//                   <select
//                     className="w-full px-3 py-2 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-purple-500"
//                     value={form.projectId}
//                     onChange={e => setForm({ ...form, projectId: e.target.value })}
//                   >
//                     <option value="">Select project...</option>
//                     {projects.map(p => (
//                       <option key={p.id} value={p.id}>{p.name}</option>
//                     ))}
//                   </select>
//                 ) : (
//                   <div className="space-y-2">
//                     <input
//                       className="w-full px-3 py-2 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-purple-500"
//                       placeholder="Project name *"
//                       value={form.newProjectName}
//                       onChange={e => setForm({ ...form, newProjectName: e.target.value })}
//                     />
//                     <input
//                       className="w-full px-3 py-2 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-purple-500"
//                       placeholder="Domain (e.g. HR, Finance)"
//                       value={form.newProjectDomain}
//                       onChange={e => setForm({ ...form, newProjectDomain: e.target.value })}
//                     />
//                   </div>
//                 )}
//               </div>

//               {/* URL Input */}
//               {inputMethod === 'url' && (
//                 <div>
//                   <label className="text-sm font-medium text-slate-600">Video URL</label>
//                   <input
//                     className="w-full mt-1 px-3 py-2 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-purple-500"
//                     placeholder="https://... or Google Drive link"
//                     value={form.videoUrl}
//                     onChange={e => setForm({ ...form, videoUrl: e.target.value })}
//                   />
//                   {/* URL Tips */}
//                   <div className="mt-2 space-y-1">
//                     <p className="text-xs font-medium text-slate-500">Supported formats:</p>
//                     <p className="text-xs text-slate-400">✅ Google Drive: drive.google.com/file/d/...</p>
//                     <p className="text-xs text-slate-400">✅ YouTube: youtube.com/watch?v=...</p>
//                     <p className="text-xs text-slate-400">✅ Direct MP4: https://example.com/video.mp4</p>
//                     <p className="text-xs text-slate-400">✅ OneDrive: 1drv.ms/... (add ?download=1)</p>
//                   </div>
//                 </div>
//               )}

//               {/* Local File Input */}
//               {inputMethod === 'local' && (
//                 <div>
//                   <label className="text-sm font-medium text-slate-600">Local File Path</label>
//                   <input
//                     className="w-full mt-1 px-3 py-2 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-purple-500"
//                     placeholder="D:/Videos/meeting.mp4"
//                     value={form.videoFilePath}
//                     onChange={e => setForm({ ...form, videoFilePath: e.target.value })}
//                   />
//                   <div className="mt-2 space-y-1">
//                     <p className="text-xs font-medium text-slate-500">Tips:</p>
//                     <p className="text-xs text-slate-400">✅ Use forward slashes: D:/Videos/file.mp4</p>
//                     <p className="text-xs text-slate-400">✅ Supported: MP4, MOV, AVI, MKV, MP3, WAV</p>
//                     <p className="text-xs text-slate-400">⚠️ Long videos may take several minutes</p>
//                   </div>
//                 </div>
//               )}

//               {/* Submit */}
//               <button
//                 type="submit"
//                 disabled={loading}
//                 className="w-full bg-purple-600 text-white py-3 rounded-lg font-medium hover:bg-purple-700 disabled:opacity-50 transition-colors flex items-center justify-center gap-2"
//               >
//                 {loading ? (
//                   <>
//                     <div className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin" />
//                     Processing...
//                   </>
//                 ) : (
//                   <>
//                     <Video size={16} />
//                     Analyze Video
//                   </>
//                 )}
//               </button>

//             </form>

//             {/* Processing Steps — shown while loading */}
//             {loading && (
//               <div className="mt-5 pt-5 border-t border-slate-100">
//                 <p className="text-xs font-medium text-slate-500 mb-3">Processing steps:</p>
//                 <div className="space-y-2">
//                   {[
//                     { step: '1', label: 'Downloading / reading video', icon: '⬇️' },
//                     { step: '2', label: 'Extracting audio with FFmpeg', icon: '🎵' },
//                     { step: '3', label: 'Splitting into chunks',        icon: '✂️' },
//                     { step: '4', label: 'Transcribing with Groq Whisper', icon: '🎙️' },
//                     { step: '5', label: 'Analyzing with ARIA',          icon: '🤖' },
//                     { step: '6', label: 'Saving to database',           icon: '💾' },
//                   ].map(s => (
//                     <div key={s.step} className="flex items-center gap-2 text-xs text-slate-400">
//                       <span>{s.icon}</span>
//                       <span>Step {s.step}: {s.label}</span>
//                     </div>
//                   ))}
//                 </div>
//               </div>
//             )}
//           </div>
//         </div>

//         {/* ── Right — Results (3 cols) ─────────────────────── */}
//         <div className="col-span-3 space-y-4">
//           {loading ? (
//             <div className="bg-white rounded-xl shadow-sm border border-slate-100 h-96 flex items-center justify-center">
//               <Loader text="ARIA is processing your video..." />
//             </div>
//           // ) : result ? (
//           //   <>
//           //     {/* ── Transcript Box ────────────────────────── */}
//           //     {result.transcript && (
//           //       <div className="bg-white rounded-xl shadow-sm border border-slate-100 overflow-hidden">
//           //         <button
//           //           onClick={() => setShowTranscript(!showTranscript)}
//           //           className="w-full flex items-center justify-between px-5 py-3 hover:bg-slate-50 transition-colors"
//           //         >
//           //           <div className="flex items-center gap-2">
//           //             <span className="text-sm font-semibold text-slate-700">
//           //               🎙️ Transcript
//           //             </span>
//           //             <span className="bg-slate-100 text-slate-500 text-xs px-2 py-0.5 rounded-full">
//           //               {result.wordCount} words
//           //             </span>
//           //           </div>
//           //           {showTranscript
//           //             ? <ChevronUp size={16} className="text-slate-400" />
//           //             : <ChevronDown size={16} className="text-slate-400" />
//           //           }
//           //         </button>

//           //         {showTranscript && (
//           //           <div className="border-t border-slate-100">
//           //             {/* Transcript lines */}
//           //             <div className="p-4 max-h-64 overflow-y-auto space-y-2 bg-slate-50">
//           //               {result.transcript.split('\n').filter(Boolean).map((line, i) => {
//           //                 // Parse "User A [0:14]: text" format
//           //                 const match = line.match(/^(.*?)\s*\[(\d+:\d+)\]:\s*(.*)$/);
//           //                 if (match) {
//           //                   const [, speaker, time, text] = match;
//           //                   return (
//           //                     <div key={i} className="flex gap-3">
//           //                       <div className="flex-shrink-0 text-right w-28">
//           //                         <span className="text-xs font-semibold text-purple-600">
//           //                           {speaker.trim()}
//           //                         </span>
//           //                         <span className="block text-xs text-slate-400">{time}</span>
//           //                       </div>
//           //                       <div className="flex-1 bg-white rounded-lg px-3 py-2 text-sm text-slate-600 border border-slate-100">
//           //                         {text}
//           //                       </div>
//           //                     </div>
//           //                   );
//           //                 }
//           //                 // Plain line
//           //                 return (
//           //                   <p key={i} className="text-sm text-slate-600 px-2">{line}</p>
//           //                 );
//           //               })}
//           //             </div>

//           //             {/* Copy transcript button */}
//           //             <div className="px-4 py-2 border-t border-slate-100 flex justify-end">
//           //               <button
//           //                 onClick={() => {
//           //                   navigator.clipboard.writeText(result.transcript);
//           //                   toast.success('Transcript copied!');
//           //                 }}
//           //                 className="text-xs text-blue-600 hover:underline"
//           //               >
//           //                 Copy transcript
//           //               </button>
//           //             </div>
//           //           </div>
//           //         )}
//           //       </div>
//           //     )}

//           //     {/* ── Analysis Result ────────────────────────── */}
//           //     {result.analysis && (
//           //       <AnalysisResult data={result.analysis} />
//           //     )}
//           //   </>

// ) : result ? (
//   <>
//     {/* ── Transcript Box ────────────────────────── */}
//     {result.transcript && (
//       <div className="bg-white rounded-xl shadow-sm border border-slate-100 overflow-hidden">
//         <button
//           onClick={() => setShowTranscript(!showTranscript)}
//           className="w-full flex items-center justify-between px-5 py-3 hover:bg-slate-50 transition-colors"
//         >
//           <div className="flex items-center gap-2">
//             <span className="text-sm font-semibold text-slate-700">
//               🎙️ Transcript
//             </span>
//             <span className="bg-slate-100 text-slate-500 text-xs px-2 py-0.5 rounded-full">
//               {result.wordCount || 0} words
//             </span>
//           </div>
//           {showTranscript
//             ? <ChevronUp size={16} className="text-slate-400" />
//             : <ChevronDown size={16} className="text-slate-400" />
//           }
//         </button>

//         {showTranscript && (
//           <div className="border-t border-slate-100">
//             <div className="p-4 max-h-64 overflow-y-auto space-y-2 bg-slate-50">
//               {(result.transcript || '').split('\n').filter(Boolean).map((line, i) => {
//                 const match = line.match(/^(.*?)\s*\[(\d+:\d+)\]:\s*(.*)$/);
//                 if (match) {
//                   const [, speaker, time, text] = match;
//                   return (
//                     <div key={i} className="flex gap-3">
//                       <div className="flex-shrink-0 text-right w-28">
//                         <span className="text-xs font-semibold text-purple-600">
//                           {speaker.trim()}
//                         </span>
//                         <span className="block text-xs text-slate-400">{time}</span>
//                       </div>
//                       <div className="flex-1 bg-white rounded-lg px-3 py-2 text-sm text-slate-600 border border-slate-100">
//                         {text}
//                       </div>
//                     </div>
//                   );
//                 }
//                 return (
//                   <p key={i} className="text-sm text-slate-600 px-2">{line}</p>
//                 );
//               })}
//             </div>
//             <div className="px-4 py-2 border-t border-slate-100 flex justify-end">
//               <button
//                 onClick={() => {
//                   navigator.clipboard.writeText(result.transcript);
//                   toast.success('Transcript copied!');
//                 }}
//                 className="text-xs text-blue-600 hover:underline"
//               >
//                 Copy transcript
//               </button>
//             </div>
//           </div>
//         )}
//       </div>
//     )}

//     {/* ── Error Message ──────────────────────────── */}
//     {result.error && (
//       <div className="bg-red-50 border border-red-200 rounded-xl p-4">
//         <p className="text-sm font-semibold text-red-700">❌ Error</p>
//         <p className="text-sm text-red-600 mt-1">{result.error}</p>
//       </div>
//     )}

//     {/* ── Analysis Result ────────────────────────── */}
//     {result.analysis && !result.analysis.error ? (
//       <AnalysisResult data={result.analysis} />
//     ) : result.analysis?.error ? (
//       <div className="bg-red-50 border border-red-200 rounded-xl p-4">
//         <p className="text-sm font-semibold text-red-700">❌ Analysis Error</p>
//         <p className="text-sm text-red-600 mt-1">{result.analysis.error}</p>
//       </div>
//     ) : !result.transcript ? (
//       <div className="bg-amber-50 border border-amber-200 rounded-xl p-4">
//         <p className="text-sm font-semibold text-amber-700">⚠️ No result received</p>
//         <p className="text-sm text-amber-600 mt-1">
//           The video may still be processing. Check the backend logs.
//         </p>
//       </div>
//     ) : null}
//   </>


//           ) : (
//             /* Empty State */
//             <div className="bg-white rounded-xl shadow-sm border border-slate-100 h-96 flex flex-col items-center justify-center text-center">
//               <div className="w-16 h-16 bg-purple-100 rounded-2xl flex items-center justify-center mb-4">
//                 <Video size={28} className="text-purple-500" />
//               </div>
//               <p className="font-semibold text-slate-700">Ready to analyze</p>
//               <p className="text-slate-400 text-sm mt-1 max-w-xs">
//                 Enter a video URL or local file path and click Analyze Video
//               </p>
//               <div className="mt-6 grid grid-cols-3 gap-3 text-xs text-slate-400">
//                 <div className="bg-slate-50 rounded-lg p-3">
//                   <p className="text-lg mb-1">🎥</p>
//                   <p>Video transcribed automatically</p>
//                 </div>
//                 <div className="bg-slate-50 rounded-lg p-3">
//                   <p className="text-lg mb-1">🎙️</p>
//                   <p>Speaker turns detected</p>
//                 </div>
//                 <div className="bg-slate-50 rounded-lg p-3">
//                   <p className="text-lg mb-1">🤖</p>
//                   <p>Requirements extracted by ARIA</p>
//                 </div>
//               </div>
//             </div>
//           )}
//         </div>

//       </div>
//     </div>
//   );
// }


import { useState, useEffect } from 'react';
import { analysisService } from '../services/analysisService';
import { projectService } from '../services/projectService';
import toast from 'react-hot-toast';
import Loader from '../components/UI/Loader';
import AnalysisResult from './AnalysisResult';
import { Video, Link, FolderOpen, ChevronDown, ChevronUp } from 'lucide-react';

export default function VideoAnalyze() {
  const [projects, setProjects]             = useState([]);
  const [loading, setLoading]               = useState(false);
  const [result, setResult]                 = useState(null);
  const [showTranscript, setShowTranscript] = useState(true);
  const [inputMethod, setInputMethod]       = useState('url');
  const [form, setForm] = useState({
    projectId:        '',
    newProjectName:   '',
    newProjectDomain: '',
    videoUrl:         '',
    videoFilePath:    '',
    useNewProject:    false,
  });

  useEffect(() => {
    projectService.getAll()
      .then(r => setProjects(r.data))
      .catch(() => {});
  }, []);

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!form.useNewProject && !form.projectId)
      return toast.error('Please select a project');
    if (form.useNewProject && !form.newProjectName.trim())
      return toast.error('Please enter a project name');
    if (inputMethod === 'url' && !form.videoUrl.trim())
      return toast.error('Please enter a video URL');
    if (inputMethod === 'local' && !form.videoFilePath.trim())
      return toast.error('Please enter a file path');

    setLoading(true);
    setResult(null);

    try {
      const base = form.useNewProject
        ? { newProjectName: form.newProjectName, newProjectDomain: form.newProjectDomain }
        : { projectId: parseInt(form.projectId) };

      const payload = {
        ...base,
        videoUrl:      inputMethod === 'url'   ? form.videoUrl     : null,
        videoFilePath: inputMethod === 'local' ? form.videoFilePath : null,
      };

      console.log('Sending payload:', payload);
      const res = await analysisService.analyzeVideo(payload);
      console.log('Response received:', res.data);
      setResult(res.data);
      toast.success('Analysis complete!');
    } catch (err) {
      console.error('Error:', err);
      toast.error(err.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="p-8">

      {/* Header */}
      <div className="mb-6">
        <h1 className="text-2xl font-bold text-slate-800 flex items-center gap-2">
          <Video size={24} className="text-purple-600" />
          Video Analysis
        </h1>
        <p className="text-slate-500 text-sm mt-1">
          Upload a meeting recording — ARIA will transcribe and analyze it automatically
        </p>
      </div>

      <div className="grid grid-cols-5 gap-6">

        {/* ── Left Form ── */}
        <div className="col-span-2">
          <div className="bg-white rounded-xl shadow-sm border border-slate-100 p-6">

            {/* Input Method Toggle */}
            <div className="mb-4">
              <p className="text-sm font-medium text-slate-600 mb-2">Input Method</p>
              <div className="grid grid-cols-2 gap-2">
                <button
                  type="button"
                  onClick={() => setInputMethod('url')}
                  className={`flex items-center justify-center gap-2 py-2.5 rounded-lg text-sm font-medium transition-all ${
                    inputMethod === 'url'
                      ? 'bg-purple-600 text-white shadow-sm'
                      : 'bg-slate-100 text-slate-600 hover:bg-slate-200'
                  }`}
                >
                  <Link size={15} /> Video URL
                </button>
                <button
                  type="button"
                  onClick={() => setInputMethod('local')}
                  className={`flex items-center justify-center gap-2 py-2.5 rounded-lg text-sm font-medium transition-all ${
                    inputMethod === 'local'
                      ? 'bg-purple-600 text-white shadow-sm'
                      : 'bg-slate-100 text-slate-600 hover:bg-slate-200'
                  }`}
                >
                  <FolderOpen size={15} /> Local File
                </button>
              </div>
            </div>

            {/* ── FORM ── */}
            <form onSubmit={handleSubmit} className="space-y-4">

              {/* Project Selection */}
              <div>
                <p className="text-sm font-medium text-slate-600 mb-2">Project</p>
                <div className="flex gap-2 mb-2">
                  <button
                    type="button"
                    onClick={() => setForm({ ...form, useNewProject: false })}
                    className={`text-xs px-3 py-1.5 rounded-lg font-medium transition-colors ${
                      !form.useNewProject ? 'bg-blue-600 text-white' : 'bg-slate-100 text-slate-600'
                    }`}
                  >
                    Existing
                  </button>
                  <button
                    type="button"
                    onClick={() => setForm({ ...form, useNewProject: true })}
                    className={`text-xs px-3 py-1.5 rounded-lg font-medium transition-colors ${
                      form.useNewProject ? 'bg-blue-600 text-white' : 'bg-slate-100 text-slate-600'
                    }`}
                  >
                    + New Project
                  </button>
                </div>

                {!form.useNewProject ? (
                  <select
                    className="w-full px-3 py-2 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-purple-500"
                    value={form.projectId}
                    onChange={e => setForm({ ...form, projectId: e.target.value })}
                  >
                    <option value="">Select project...</option>
                    {projects.map(p => (
                      <option key={p.id} value={p.id}>{p.name}</option>
                    ))}
                  </select>
                ) : (
                  <div className="space-y-2">
                    <input
                      className="w-full px-3 py-2 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-purple-500"
                      placeholder="Project name *"
                      value={form.newProjectName}
                      onChange={e => setForm({ ...form, newProjectName: e.target.value })}
                    />
                    <input
                      className="w-full px-3 py-2 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-purple-500"
                      placeholder="Domain (e.g. HR, Finance)"
                      value={form.newProjectDomain}
                      onChange={e => setForm({ ...form, newProjectDomain: e.target.value })}
                    />
                  </div>
                )}
              </div>

              {/* URL Input */}
              {inputMethod === 'url' && (
                <div>
                  <label className="text-sm font-medium text-slate-600">Video URL</label>
                  <input
                    className="w-full mt-1 px-3 py-2 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-purple-500"
                    placeholder="https://... or Google Drive link"
                    value={form.videoUrl}
                    onChange={e => setForm({ ...form, videoUrl: e.target.value })}
                  />
                  <div className="mt-2 space-y-0.5">
                    <p className="text-xs font-medium text-slate-500">Supported:</p>
                    <p className="text-xs text-slate-400">✅ Google Drive: drive.google.com/file/d/...</p>
                    <p className="text-xs text-slate-400">✅ YouTube: youtube.com/watch?v=...</p>
                    <p className="text-xs text-slate-400">✅ Direct MP4 URL</p>
                  </div>
                </div>
              )}

              {/* Local File Input */}
              {inputMethod === 'local' && (
                <div>
                  <label className="text-sm font-medium text-slate-600">Local File Path</label>
                  <input
                    className="w-full mt-1 px-3 py-2 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-purple-500"
                    placeholder="D:/Videos/meeting.mp4"
                    value={form.videoFilePath}
                    onChange={e => setForm({ ...form, videoFilePath: e.target.value })}
                  />
                  <div className="mt-2 bg-amber-50 border border-amber-100 rounded-lg p-2">
                    <p className="text-xs text-amber-700 font-medium">📁 How to get exact path:</p>
                    <p className="text-xs text-amber-600 mt-0.5">
                      Hold Shift + Right click file → "Copy as path"<br />
                      Paste here, remove quotes, use forward slashes
                    </p>
                  </div>
                  <div className="mt-1 space-y-0.5">
                    <p className="text-xs text-slate-400">✅ Supported: MP4, MOV, AVI, MKV, MP3, WAV</p>
                    <p className="text-xs text-slate-400">⚠️ Long videos take several minutes</p>
                  </div>
                </div>
              )}

              {/* Submit Button */}
              <button
                type="submit"
                disabled={loading}
                className="w-full bg-purple-600 text-white py-3 rounded-lg font-medium hover:bg-purple-700 disabled:opacity-50 transition-colors flex items-center justify-center gap-2"
              >
                {loading ? (
                  <>
                    <div className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin" />
                    Processing...
                  </>
                ) : (
                  <>
                    <Video size={16} />
                    Analyze Video
                  </>
                )}
              </button>

            </form>
            {/* ── END FORM ── */}

            {/* Processing Steps */}
            {loading && (
              <div className="mt-4 pt-4 border-t border-slate-100">
                <p className="text-xs font-medium text-slate-500 mb-2">Processing steps:</p>
                <div className="space-y-1.5">
                  {[
                    { icon: '⬇️', label: 'Downloading / reading video' },
                    { icon: '🎵', label: 'Extracting audio with FFmpeg' },
                    { icon: '✂️', label: 'Splitting into 4-min chunks' },
                    { icon: '🎙️', label: 'Transcribing with Groq Whisper' },
                    { icon: '🤖', label: 'Analyzing with ARIA' },
                    { icon: '💾', label: 'Saving to database' },
                  ].map((s, i) => (
                    <div key={i} className="flex items-center gap-2 text-xs text-slate-400">
                      <span>{s.icon}</span>
                      <span>{s.label}</span>
                    </div>
                  ))}
                </div>
              </div>
            )}

          </div>
        </div>

        {/* ── Right Results ── */}
        <div className="col-span-3 space-y-4">

          {/* Loading state */}
          {loading && (
            <div className="bg-white rounded-xl shadow-sm border border-slate-100 h-96 flex items-center justify-center">
              <Loader text="ARIA is processing your video..." />
            </div>
          )}

          {/* Result state */}
          {!loading && result && (
            <>
              {/* Transcript */}
              {result.transcript && (
                <div className="bg-white rounded-xl shadow-sm border border-slate-100 overflow-hidden">
                  <button
                    onClick={() => setShowTranscript(!showTranscript)}
                    className="w-full flex items-center justify-between px-5 py-3 hover:bg-slate-50 transition-colors"
                  >
                    <div className="flex items-center gap-2">
                      <span className="text-sm font-semibold text-slate-700">🎙️ Transcript</span>
                      <span className="bg-slate-100 text-slate-500 text-xs px-2 py-0.5 rounded-full">
                        {result.wordCount || 0} words
                      </span>
                    </div>
                    {showTranscript
                      ? <ChevronUp size={16} className="text-slate-400" />
                      : <ChevronDown size={16} className="text-slate-400" />
                    }
                  </button>

                  {showTranscript && (
                    <div className="border-t border-slate-100">
                      <div className="p-4 max-h-64 overflow-y-auto space-y-2 bg-slate-50">
                        {String(result.transcript).split('\n').filter(Boolean).map((line, i) => {
                          const match = line.match(/^(.*?)\s*\[(\d+:\d+)\]:\s*(.*)$/);
                          if (match) {
                            const [, speaker, time, text] = match;
                            return (
                              <div key={i} className="flex gap-3">
                                <div className="flex-shrink-0 text-right w-28">
                                  <span className="text-xs font-semibold text-purple-600 block truncate">
                                    {speaker.trim()}
                                  </span>
                                  <span className="text-xs text-slate-400">{time}</span>
                                </div>
                                <div className="flex-1 bg-white rounded-lg px-3 py-2 text-sm text-slate-600 border border-slate-100">
                                  {text}
                                </div>
                              </div>
                            );
                          }
                          return <p key={i} className="text-sm text-slate-600 px-2">{line}</p>;
                        })}
                      </div>
                      <div className="px-4 py-2 border-t border-slate-100 flex justify-end">
                        <button
                          onClick={() => {
                            navigator.clipboard.writeText(result.transcript);
                            toast.success('Transcript copied!');
                          }}
                          className="text-xs text-blue-600 hover:underline"
                        >
                          Copy transcript
                        </button>
                      </div>
                    </div>
                  )}
                </div>
              )}

              {/* Backend error */}
              {result.error && (
                <div className="bg-red-50 border border-red-200 rounded-xl p-4">
                  <p className="text-sm font-semibold text-red-700">❌ Error</p>
                  <p className="text-sm text-red-600 mt-1">{result.error}</p>
                </div>
              )}

              {/* Analysis */}
              {result.analysis && !result.analysis.error && (
                <AnalysisResult data={result.analysis} />
              )}

              {result.analysis?.error && (
                <div className="bg-red-50 border border-red-200 rounded-xl p-4">
                  <p className="text-sm font-semibold text-red-700">❌ Analysis Error</p>
                  <p className="text-sm text-red-600 mt-1">{result.analysis.error}</p>
                </div>
              )}

              {/* No transcript and no analysis */}
              {!result.transcript && !result.analysis && !result.error && (
                <div className="bg-amber-50 border border-amber-200 rounded-xl p-4">
                  <p className="text-sm font-semibold text-amber-700">⚠️ No result received</p>
                  <p className="text-sm text-amber-600 mt-1">
                    Check the backend logs for details.
                  </p>
                </div>
              )}
            </>
          )}

          {/* Empty state */}
          {!loading && !result && (
            <div className="bg-white rounded-xl shadow-sm border border-slate-100 h-96 flex flex-col items-center justify-center text-center">
              <div className="w-16 h-16 bg-purple-100 rounded-2xl flex items-center justify-center mb-4">
                <Video size={28} className="text-purple-500" />
              </div>
              <p className="font-semibold text-slate-700">Ready to analyze</p>
              <p className="text-slate-400 text-sm mt-1 max-w-xs">
                Enter a video URL or local file path and click Analyze Video
              </p>
              <div className="mt-6 grid grid-cols-3 gap-3 text-xs text-slate-400">
                <div className="bg-slate-50 rounded-lg p-3">
                  <p className="text-lg mb-1">🎥</p>
                  <p>Video transcribed automatically</p>
                </div>
                <div className="bg-slate-50 rounded-lg p-3">
                  <p className="text-lg mb-1">🎙️</p>
                  <p>Speaker turns detected</p>
                </div>
                <div className="bg-slate-50 rounded-lg p-3">
                  <p className="text-lg mb-1">🤖</p>
                  <p>Requirements extracted by ARIA</p>
                </div>
              </div>
            </div>
          )}

        </div>
      </div>
    </div>
  );
}
