import axiosClient from '../api/axiosClient';

const blogService = {
    // Lấy tất cả bài viết
    getAllPosts: () => {
        return axiosClient.get('/Posts');
    },

    // Lấy bài viết theo chuyên mục
    getPostByCategoryId: (categoryId) => {
        return axiosClient.get(`/Posts/Category/${categoryId}`);
    },

    // Lấy danh sách chuyên mục
    getBlogCategories: () => {
        return axiosClient.get('/Categories');
    }
};

export default blogService;