import axiosInstance from '../api/axiosInstance';

export const projectService = {
  getAll: () =>
    axiosInstance.get('/Project'),

  getById: (id) =>
    axiosInstance.get(`/Project/${id}`),

  create: (data) =>
    axiosInstance.post('/Project', data),
};