import axiosInstance from '../api/axiosInstance';

export const analysisService = {
  analyze: (data) =>
    axiosInstance.post('/Analysis/analyze', data),

  analyzeVideo: (data) =>
    axiosInstance.post('/Analysis/analyze-video', data),

  consolidate: (projectId) =>
    axiosInstance.post(`/Analysis/consolidate/${projectId}`),

  getHistory: (projectId) =>
    axiosInstance.get(`/Analysis/history/${projectId}`),

  getLatest: (projectId) =>
    axiosInstance.get(`/Analysis/latest/${projectId}`),

  getStats:     () => axiosInstance.get('/Analysis/stats'),  
};