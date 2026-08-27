// export default function History() {
//   return (
//     <div className="p-8">
//       <h1 className="text-3xl font-bold">
//         History Page
//       </h1>
//     </div>
//   );
// }

import { useState, useEffect } from 'react';
import { analysisService } from '../services/analysisService';
import { projectService } from '../services/projectService';
import toast from 'react-hot-toast';
import Loader from '../components/UI/Loader';
import EmptyState from '../components/UI/EmptyState';
import { History as HistoryIcon, MessageSquare, Video, Calendar, ChevronRight } from 'lucide-react';

export default function History() {
  const [projects, setProjects]         = useState([]);
  const [history, setHistory]           = useState([]);
  const [selected, setSelected]         = useState('');
  const [loading, setLoading]           = useState(false);
  const [selectedConversation, setSelectedConversation] = useState(null);
  const [searchQuery, setSearchQuery]   = useState('');

  useEffect(() => {
    projectService.getAll()
      .then(r => setProjects(r.data))
      .catch(() => {});
  }, []);

  const loadHistory = async (projectId) => {
    setSelected(projectId);
    setHistory([]);
    setSelectedConversation(null);
    setSearchQuery('');
    if (!projectId) return;

    setLoading(true);
    try {
      const res = await analysisService.getHistory(projectId);
      setHistory(res.data);
    } catch {
      toast.error('Failed to load history');
    } finally {
      setLoading(false);
    }
  };

  // Filter by search
  const filtered = history.filter(h =>
    h.title?.toLowerCase().includes(searchQuery.toLowerCase()) ||
    h.projectTitle?.toLowerCase().includes(searchQuery.toLowerCase())
  );

  // Format date nicely
  const formatDate = (dateStr) => {
    const date = new Date(dateStr);
    return {
      date: date.toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' }),
      time: date.toLocaleTimeString('en-GB', { hour: '2-digit', minute: '2-digit' }),
    };
  };

  // Clean title — remove "User A [0:00]: you" junk
  const cleanTitle = (title) => {
    if (!title) return 'Untitled Conversation';
    // Remove speaker labels from start
    const cleaned = title
      .replace(/^(User|Speaker)\s+[A-Z]\s*(\[\d+:\d+\])?\s*:\s*/gi, '')
      .replace(/^you\s*/i, '')
      .trim();
    return cleaned.length > 80
      ? cleaned.substring(0, 80) + '...'
      : cleaned || 'Untitled Conversation';
  };

  return (
    <div className="p-8">
      {/* Header */}
      <div className="mb-6">
        <h1 className="text-2xl font-bold text-slate-800 flex items-center gap-2">
          <HistoryIcon size={24} className="text-blue-600" />
          History
        </h1>
        <p className="text-slate-500 text-sm mt-1">
          View all past analyses for each project
        </p>
      </div>

      {/* Project Selector + Search */}
      <div className="bg-white rounded-xl shadow-sm border border-slate-100 p-5 mb-6">
        <div className="flex gap-4">
          <div className="flex-1">
            <label className="text-sm font-medium text-slate-600">Select Project</label>
            <select
              className="w-full mt-1 px-3 py-2 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
              value={selected}
              onChange={e => loadHistory(e.target.value)}
            >
              <option value="">Choose a project...</option>
              {projects.map(p => (
                <option key={p.id} value={p.id}>{p.name}</option>
              ))}
            </select>
          </div>

          {history.length > 0 && (
            <div className="flex-1">
              <label className="text-sm font-medium text-slate-600">Search</label>
              <input
                className="w-full mt-1 px-3 py-2 border border-slate-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                placeholder="Search conversations..."
                value={searchQuery}
                onChange={e => setSearchQuery(e.target.value)}
              />
            </div>
          )}

          {history.length > 0 && (
            <div className="flex items-end">
              <div className="bg-blue-50 rounded-lg px-4 py-2 text-center">
                <p className="text-2xl font-bold text-blue-600">{history.length}</p>
                <p className="text-xs text-slate-500">Total sessions</p>
              </div>
            </div>
          )}
        </div>
      </div>

      {/* Content */}
      {loading ? (
        <Loader text="Loading history..." />
      ) : !selected ? (
        <EmptyState
          icon="📂"
          title="Select a project"
          description="Choose a project above to view all its analysis history"
        />
      ) : filtered.length === 0 ? (
        <EmptyState
          icon="📭"
          title="No history found"
          description={searchQuery ? 'No results match your search' : 'No analyses found for this project'}
        />
      ) : (
        <div className="grid grid-cols-3 gap-5">

          {/* ── Left: Conversation List ──────────────────── */}
          <div className="col-span-1">
            <div className="bg-white rounded-xl shadow-sm border border-slate-100 overflow-hidden">
              <div className="px-4 py-3 border-b border-slate-50 bg-slate-50">
                <p className="text-xs font-semibold text-slate-500 uppercase tracking-wide">
                  {filtered.length} Conversations
                </p>
              </div>

              <div className="divide-y divide-slate-50 max-h-[65vh] overflow-y-auto">
                {filtered.map((h, i) => {
                  const { date, time } = formatDate(h.createdAt);
                  const isSelected = selectedConversation?.conversationId === h.conversationId;

                  return (
                    <button
                      key={h.conversationId}
                      onClick={() => setSelectedConversation(h)}
                      className={`w-full text-left px-4 py-3.5 hover:bg-slate-50 transition-colors flex items-start gap-3 ${
                        isSelected ? 'bg-blue-50 border-l-2 border-blue-500' : ''
                      }`}
                    >
                      {/* Session number */}
                      <div className={`w-7 h-7 rounded-full flex items-center justify-center flex-shrink-0 mt-0.5 ${
                        isSelected ? 'bg-blue-600' : 'bg-slate-100'
                      }`}>
                        <span className={`text-xs font-bold ${
                          isSelected ? 'text-white' : 'text-slate-500'
                        }`}>
                          {i + 1}
                        </span>
                      </div>

                      <div className="flex-1 min-w-0">
                        {/* Project title */}
                        {h.projectTitle ? (
                          <p className="text-xs font-semibold text-blue-600 mb-0.5 truncate">
                            {h.projectTitle}
                          </p>
                        ) : (
                          <p className="text-xs text-slate-300 mb-0.5">No title extracted</p>
                        )}

                        {/* Conversation preview */}
                        <p className="text-sm text-slate-700 font-medium truncate">
                          {cleanTitle(h.title)}
                        </p>

                        {/* Date + time */}
                        <div className="flex items-center gap-2 mt-1.5">
                          <Calendar size={11} className="text-slate-300" />
                          <span className="text-xs text-slate-400">{date}</span>
                          <span className="text-xs text-slate-300">{time}</span>
                        </div>
                      </div>

                      <ChevronRight size={14} className={`flex-shrink-0 mt-1 ${
                        isSelected ? 'text-blue-500' : 'text-slate-300'
                      }`} />
                    </button>
                  );
                })}
              </div>
            </div>
          </div>

          {/* ── Right: Conversation Detail ───────────────── */}
          <div className="col-span-2">
            {selectedConversation ? (
              <ConversationDetail conversation={selectedConversation} />
            ) : (
              <div className="bg-white rounded-xl shadow-sm border border-slate-100 h-full flex flex-col items-center justify-center text-center p-10">
                <div className="w-14 h-14 bg-blue-100 rounded-2xl flex items-center justify-center mb-4">
                  <MessageSquare size={24} className="text-blue-500" />
                </div>
                <p className="font-semibold text-slate-700">Select a conversation</p>
                <p className="text-slate-400 text-sm mt-1">
                  Click any session on the left to view its details
                </p>
              </div>
            )}
          </div>

        </div>
      )}
    </div>
  );
}

// ── Conversation Detail Component ──────────────────────────────────────
function ConversationDetail({ conversation }) {
  const { date, time } = formatDateFn(conversation.createdAt);

  return (
    <div className="bg-white rounded-xl shadow-sm border border-slate-100 overflow-hidden">
      {/* Detail Header */}
      <div className="px-5 py-4 border-b border-slate-100 bg-gradient-to-r from-blue-50 to-white">
        <div className="flex items-start justify-between">
          <div>
            <h3 className="font-bold text-slate-800">
              {conversation.projectTitle || 'Analysis Session'}
            </h3>
            <div className="flex items-center gap-3 mt-1">
              <div className="flex items-center gap-1 text-xs text-slate-400">
                <Calendar size={11} />
                {date} at {time}
              </div>
              <span className="text-xs bg-blue-100 text-blue-600 px-2 py-0.5 rounded-full font-medium">
                ID: #{conversation.conversationId}
              </span>
              {conversation.sourceType && (
                <span className="text-xs bg-purple-100 text-purple-600 px-2 py-0.5 rounded-full font-medium">
                  {conversation.sourceType || 'text'}
                </span>
              )}
            </div>
          </div>
        </div>
      </div>

      {/* Transcript Preview */}
      <div className="p-5 max-h-[60vh] overflow-y-auto">
        <div className="mb-4">
          <p className="text-xs font-semibold text-slate-500 uppercase tracking-wide mb-2">
            Conversation Preview
          </p>
          <div className="bg-slate-50 rounded-xl p-4 space-y-2 max-h-48 overflow-y-auto">
            {conversation.title?.split('\n').filter(Boolean).slice(0, 10).map((line, i) => {
              const match = line.match(/^(.*?)\s*(\[\d+:\d+\])?\s*:\s*(.*)$/);
              if (match) {
                const [, speaker, time, text] = match;
                return (
                  <div key={i} className="flex gap-3">
                    <div className="flex-shrink-0 w-24 text-right">
                      <span className="text-xs font-semibold text-blue-600 truncate block">
                        {speaker?.trim()}
                      </span>
                      {time && (
                        <span className="text-xs text-slate-400">{time}</span>
                      )}
                    </div>
                    <p className="text-sm text-slate-600 flex-1">{text}</p>
                  </div>
                );
              }
              return (
                <p key={i} className="text-sm text-slate-500">{line}</p>
              );
            })}
          </div>
        </div>

        {/* Info Cards */}
        <div className="grid grid-cols-3 gap-3 mt-4">
          <div className="bg-blue-50 rounded-xl p-3 text-center">
            <p className="text-lg font-bold text-blue-600">
              #{conversation.conversationId}
            </p>
            <p className="text-xs text-slate-500 mt-0.5">Conversation ID</p>
          </div>
          <div className="bg-green-50 rounded-xl p-3 text-center">
            <p className="text-lg font-bold text-green-600">{date}</p>
            <p className="text-xs text-slate-500 mt-0.5">Analyzed on</p>
          </div>
          <div className="bg-purple-50 rounded-xl p-3 text-center">
            <p className="text-lg font-bold text-purple-600">{time}</p>
            <p className="text-xs text-slate-500 mt-0.5">Time</p>
          </div>
        </div>

        {/* Note */}
        <div className="mt-4 bg-amber-50 border border-amber-100 rounded-xl p-3">
          <p className="text-xs text-amber-700">
            💡 To see full analysis results for this conversation, go to
            <strong> Consolidate</strong> to merge all sessions or
            re-analyze from the <strong>Analyze</strong> page.
          </p>
        </div>
      </div>
    </div>
  );
}

// Helper outside component
function formatDateFn(dateStr) {
  const date = new Date(dateStr);
  return {
    date: date.toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' }),
    time: date.toLocaleTimeString('en-GB', { hour: '2-digit', minute: '2-digit' }),
  };
}