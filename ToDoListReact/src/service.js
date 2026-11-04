import axios from 'axios';

axios.defaults.baseURL = "https://todo-api-a7dp.onrender.com";

axios.interceptors.response.use(
  (response) => response,
  (error) => {
    console.error("API error:", error.response?.status, error.message);
    return Promise.reject(error);
  }
);

export default {
  getTasks: async () => {
    const result = await axios.get(`/items`)
    return result.data;
  },

  addTask: async(name)=>{
    const result = await axios.post(`/items`, { name, isComplete: false });
    return result.data;
  },

  setCompleted: async(id, isComplete)=>{
    // ה-PUT ב-API דורס גם את השם, לכן שולפים את המשימה הקיימת כדי לשמור עליו
    const tasks = (await axios.get(`/items`)).data;
    const task = tasks.find(t => t.id === id);
    const result = await axios.put(`/items/${id}`, { id, name: task?.name, isComplete });
    return result.data;
  },

  deleteTask:async(id)=>{
    await axios.delete(`/items/${id}`);
  }
};
