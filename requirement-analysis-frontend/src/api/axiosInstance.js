// import axios from 'axios';

// const axiosInstance = axios.create({
//   baseURL: 'https://localhost:7262/api',
//   headers: {
//     'Content-Type': 'application/json',
//   },
// });

// axiosInstance.interceptors.response.use(
//   (response) => response,
//   (error) => {
//     const message =
//       error.response?.data?.error ||
//       error.message ||
//       'Something went wrong';

//     return Promise.reject(new Error(message));
//   }
// );

// export default axiosInstance;

import axios from 'axios';

const axiosInstance = axios.create({
  baseURL: 'https://localhost:7262/api', // ← verify this port
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 30000, // ← ADD: 30 second timeout
});

axiosInstance.interceptors.request.use(
  (config) => config,
  (error) => Promise.reject(error)
);

axiosInstance.interceptors.response.use(
  (response) => response,
  (error) => {
    // ← Better error messages
    if (error.code === 'ERR_NETWORK')
      return Promise.reject(new Error('Cannot connect to server. Make sure backend is running.'));

    if (error.code === 'ERR_CERT_AUTHORITY_INVALID')
      return Promise.reject(new Error('SSL Certificate error. Open https://localhost:7262/swagger in browser first.'));

    const message = error.response?.data?.error
      || error.response?.data?.title
      || error.message
      || 'Something went wrong';

    return Promise.reject(new Error(message));
  }
);

export default axiosInstance;