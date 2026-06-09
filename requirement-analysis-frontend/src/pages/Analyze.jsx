import { useEffect, useState } from 'react';

import toast from 'react-hot-toast';

import Loader from '../components/UI/Loader';

import AnalysisResult from './AnalysisResult';

import { projectService } from '../services/projectService';
import { analysisService } from '../services/analysisService';

export default function Analyze() {

  const [projects, setProjects] = useState([]);

  const [loading, setLoading] = useState(false);

  const [result, setResult] = useState(null);

  const [form, setForm] = useState({
    projectId: '',
    conversation: '',
  });

  // Load Projects
  useEffect(() => {

    projectService
      .getAll()
      .then((response) => {
        setProjects(response.data);
      });

  }, []);

  // Submit Analysis
  const handleSubmit = async (e) => {

    e.preventDefault();

    if (!form.projectId) {
      toast.error('Please select a project');
      return;
    }

    if (!form.conversation.trim()) {
      toast.error('Conversation is required');
      return;
    }

    try {

      setLoading(true);

      setResult(null);

      const response = await analysisService.analyze({
        projectId: parseInt(form.projectId),
        conversation: form.conversation,
      });

      setResult(response.data.analysis || response.data);

      toast.success('Analysis completed');

    } catch (error) {

      toast.error(error.message);

    } finally {

      setLoading(false);
    }
  };

  return (
    <div className="p-8">

      {/* Header */}
      <div className="mb-8">

        <h1 className="text-3xl font-bold text-slate-800">
          Analyze Conversation
        </h1>

        <p className="text-slate-500 mt-2">
          Paste conversation or meeting transcript for AI analysis
        </p>

      </div>

      <div className="grid grid-cols-2 gap-6">

        {/* Left */}
        <div className="bg-white rounded-2xl border border-slate-200 shadow-sm p-6">

          <form
            onSubmit={handleSubmit}
            className="space-y-5"
          >

            {/* Project */}
            <div>

              <label className="block text-sm font-medium text-slate-600 mb-2">
                Select Project
              </label>

              <select
                value={form.projectId}
                onChange={(e) =>
                  setForm({
                    ...form,
                    projectId: e.target.value,
                  })
                }
                className="w-full border border-slate-300 rounded-xl px-4 py-3 outline-none focus:ring-2 focus:ring-blue-500"
              >
                <option value="">
                  Choose project
                </option>

                {projects.map((project) => (
                  <option
                    key={project.id}
                    value={project.id}
                  >
                    {project.name}
                  </option>
                ))}

              </select>

            </div>

            {/* Conversation */}
            <div>

              <label className="block text-sm font-medium text-slate-600 mb-2">
                Conversation
              </label>

              <textarea
                rows={16}
                value={form.conversation}
                onChange={(e) =>
                  setForm({
                    ...form,
                    conversation: e.target.value,
                  })
                }
                className="w-full border border-slate-300 rounded-xl px-4 py-3 outline-none focus:ring-2 focus:ring-blue-500 resize-none"
                placeholder="Paste your meeting transcript or conversation here..."
              />

            </div>

            {/* Button */}
            <button
              type="submit"
              disabled={loading}
              className="w-full bg-blue-600 hover:bg-blue-700 text-white py-3 rounded-xl transition font-medium disabled:opacity-50"
            >
              {loading
                ? 'Analyzing...'
                : 'Analyze Conversation'}
            </button>

          </form>

        </div>

        {/* Right */}
        <div>

          {loading ? (

            <div className="bg-white rounded-2xl border border-slate-200 shadow-sm h-full flex items-center justify-center">

              <Loader text="AI is analyzing conversation..." />

            </div>

          ) : result ? (

            <AnalysisResult data={result} />

          ) : (

            <div className="bg-white rounded-2xl border border-slate-200 shadow-sm h-full flex items-center justify-center">

              <div className="text-center">

                <div className="text-6xl mb-4">
                  🤖
                </div>

                <h2 className="text-xl font-semibold text-slate-700">
                  AI Analysis Result
                </h2>

                <p className="text-slate-500 mt-2">
                  Results will appear here
                </p>

              </div>

            </div>

          )}

        </div>

      </div>

    </div>
  );
}