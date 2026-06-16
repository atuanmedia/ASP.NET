import axiosClient from '../api/axiosClient';

const categoryProductService = {
    /**
     * Hàm lấy toàn bộ danh mục SẢN PHẨM từ Backend
     * Endpoint này kết nối tới CategoryProductController trong ASP.NET Core
     * URL: GET /api/CategoriesProduct
     */
    getAllCategoryProducts: () => {
        const url = '/CategoriesProduct';
        return axiosClient.get(url);
    },

    /**
     * Hàm lấy chi tiết một danh mục kèm theo tất cả các sản phẩm thuộc danh mục đó (Bộ lọc theo danh mục)
     * URL: GET /api/CategoriesProduct/{id}
     * @param {number|string} id - Id của danh mục cần lọc sản phẩm
     */
    getCategoryByIdWithProducts: (id) => {
        const url = `/CategoriesProduct/${id}`;
        return axiosClient.get(url);
    }
};

export default categoryProductService;