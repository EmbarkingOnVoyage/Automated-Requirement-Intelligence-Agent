import { useEffect, useState } from 'react';

import {
  FolderOpen,
  Plus
} from 'lucide-react';

import toast from 'react-hot-toast';

import Loader from '../components/UI/Loader';
import EmptyState from '../components/UI/EmptyState';

import { projectService } from '../services/projectService';

export default function Projects() {

  const [projects, setProjects] = useState([]);
  const [loading, setLoading] = useState(true);

  const [showForm, setShowForm] = useState(false);

  const [form, setForm] = useState({
    name: '',
    description: '',
    domain: '',
  });

  // Load Projects
  const loadProjects = async () => {
    try {

      setLoading(true);

      const response = await projectService.getAll();

      setProjects(response.data);

    } catch (error) {

      toast.error('Failed to load projects');

    } finally {

      setLoading(false);
    }
  };

  useEffect(() => {
    loadProjects();
  }, []);

  // Create Project
  const handleCreate = async (e) => {

    e.preventDefault();

    if (!form.name.trim()) {
      toast.error('Project name is required');
      return;
    }

    try {

      await projectService.create(form);

      toast.success('Project created successfully');

      setForm({
        name: '',
        description: '',
        domain: '',
      });

      setShowForm(false);

      loadProjects();

    } catch (error) {

      toast.error(error.message);
    }
  };

  return (
    <div className="p-8">

      {/* Header */}
      <div className="flex items-center justify-between mb-8">

        <div>
          <h1 className="text-3xl font-bold text-slate-800">
            Projects
          </h1>

          <p className="text-slate-500 mt-1">
            Manage your requirement analysis projects
          </p>
        </div>

        <button
          onClick={() => setShowForm(!showForm)}
          className="flex items-center gap-2 bg-blue-600 hover:bg-blue-700 text-white px-5 py-3 rounded-xl transition"
        >
          <Plus size={18} />

          New Project
        </button>

      </div>

      {/* Create Form */}
      {showForm && (
        <div className="bg-white rounded-2xl border border-slate-200 shadow-sm p-6 mb-8">

          <h2 className="text-xl font-semibold mb-5">
            Create Project
          </h2>

          <form
            onSubmit={handleCreate}
            className="space-y-5"
          >

            <div>
              <label className="block text-sm font-medium text-slate-600 mb-2">
                Project Name
              </label>

              <input
                type="text"
                value={form.name}
                onChange={(e) =>
                  setForm({
                    ...form,
                    name: e.target.value,
                  })
                }
                className="w-full border border-slate-300 rounded-xl px-4 py-3 outline-none focus:ring-2 focus:ring-blue-500"
                placeholder="Enter project name"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-slate-600 mb-2">
                Domain
              </label>

              <input
                type="text"
                value={form.domain}
                onChange={(e) =>
                  setForm({
                    ...form,
                    domain: e.target.value,
                  })
                }
                className="w-full border border-slate-300 rounded-xl px-4 py-3 outline-none focus:ring-2 focus:ring-blue-500"
                placeholder="e.g HR, Banking, E-Commerce"
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-slate-600 mb-2">
                Description
              </label>

              <textarea
                rows={4}
                value={form.description}
                onChange={(e) =>
                  setForm({
                    ...form,
                    description: e.target.value,
                  })
                }
                className="w-full border border-slate-300 rounded-xl px-4 py-3 outline-none focus:ring-2 focus:ring-blue-500"
                placeholder="Enter project description"
              />
            </div>

            <div className="flex gap-3">

              <button
                type="submit"
                className="bg-blue-600 hover:bg-blue-700 text-white px-5 py-3 rounded-xl transition"
              >
                Create Project
              </button>

              <button
                type="button"
                onClick={() => setShowForm(false)}
                className="bg-slate-200 hover:bg-slate-300 text-slate-700 px-5 py-3 rounded-xl transition"
              >
                Cancel
              </button>

            </div>

          </form>

        </div>
      )}

      {/* Loading */}
      {loading ? (
        <Loader text="Loading projects..." />
      ) : projects.length === 0 ? (

        <EmptyState
          title="No Projects Yet"
          description="Create your first project to begin requirement analysis"
          action={
            <button
              onClick={() => setShowForm(true)}
              className="bg-blue-600 hover:bg-blue-700 text-white px-5 py-3 rounded-xl transition"
            >
              Create Project
            </button>
          }
        />

      ) : (

        <div className="grid grid-cols-2 gap-5">

          {projects.map((project) => (

            <div
              key={project.id}
              className="bg-white rounded-2xl border border-slate-200 shadow-sm p-5 hover:shadow-md transition"
            >

              <div className="flex items-start gap-4">

                <div className="w-14 h-14 rounded-xl bg-blue-100 flex items-center justify-center">
                  <FolderOpen className="text-blue-600" />
                </div>

                <div className="flex-1">

                  <h2 className="text-lg font-semibold text-slate-800">
                    {project.name}
                  </h2>

                  <p className="text-sm text-slate-500 mt-1">
                    {project.domain || 'General'}
                  </p>

                  {project.description && (
                    <p className="text-slate-600 mt-3 text-sm">
                      {project.description}
                    </p>
                  )}

                </div>

              </div>

            </div>
          ))}

        </div>
      )}

    </div>
  );
}