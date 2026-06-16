import axiosClient from '../api/axiosClient';

const productService = {

    // ==========================================
    // SHOP API
    // ==========================================
    getShopProducts: (params = {}) => {
        return axiosClient.get('/products/shop', {
            params
        });
    },

    // ==========================================
    // GET ALL PRODUCTS
    // ==========================================
    getAllProducts: () => {
        return axiosClient.get('/products');
    },

    // ==========================================
    // GET PRODUCT DETAIL
    // ==========================================
    getProductById: (id) => {
        return axiosClient.get(`/products/${id}`);
    },

    // ==========================================
    // GET PRODUCTS BY CATEGORY
    // ==========================================
    getProductsByCategory: (categoryId) => {
        return axiosClient.get(`/products/category/${categoryId}`);
    },

    // ==========================================
    // CREATE PRODUCT
    // ==========================================
    createProduct: (formData) => {
        return axiosClient.post(
            '/products',
            formData,
            {
                headers: {
                    'Content-Type': 'multipart/form-data'
                }
            }
        );
    },

    // ==========================================
    // UPDATE PRODUCT
    // ==========================================
    updateProduct: (id, formData) => {
        return axiosClient.put(
            `/products/${id}`,
            formData,
            {
                headers: {
                    'Content-Type': 'multipart/form-data'
                }
            }
        );
    },

    // ==========================================
    // DELETE PRODUCT
    // ==========================================
    deleteProduct: (id) => {
        return axiosClient.delete(`/products/${id}`);
    }
};

export default productService;