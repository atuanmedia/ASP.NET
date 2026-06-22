import axiosClient from '../api/axiosClient';

const blogService = {
  // Lấy bài viết có phân trang + lọc theo danh mục
  getAllPosts: ({ page = 1, pageSize = 9, categoryId = null } = {}) => {
    const params = { page, pageSize };
    if (categoryId) params.categoryId = categoryId;
    return axiosClient.get('/Posts', { params });
  },

  // Chi tiết bài viết
  getPostById: (id) => axiosClient.get(`/Posts/${id}`),

  // Danh sách danh mục bài viết
  getBlogCategories: () => axiosClient.get('/Categories'),
};

export default blogService;
