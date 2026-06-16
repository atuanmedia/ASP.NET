import React, { useState, useEffect } from "react";
import { Link } from "react-router-dom"; // Import Link để chuyển hướng trang
import categoryProductService from "../../services/categoryProductService";
import "./CategoryProductList.css";

const CategoryProductList = () => {
    const [categoryProducts, setCategoryProducts] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchCategoryProducts = async () => {
            try {
                const data = await categoryProductService.getAllCategoryProducts();
                setCategoryProducts(data);
            } catch (error) {
                console.error("Lỗi khi tải danh mục sản phẩm:", error);
            } finally {
                setLoading(false);
            }
        };

        fetchCategoryProducts();
    }, []);

    const getIconCategory = (name) => {
        const lowerName = name?.toLowerCase() || "";
        if (lowerName.includes("điện thoại") || lowerName.includes("iphone")) return "fa-mobile-screen-button";
        if (lowerName.includes("máy tính bảng") || lowerName.includes("ipad")) return "fa-tablet-screen-button";
        if (lowerName.includes("laptop") || lowerName.includes("máy tính")) return "fa-laptop";
        if (lowerName.includes("tai nghe")) return "fa-headphones";
        if (lowerName.includes("sofa") || lowerName.includes("ghế")) return "fa-couch";
        if (lowerName.includes("bàn")) return "fa-table";
        if (lowerName.includes("giường")) return "fa-bed";
        return "fa-box-open"; 
    };

    if (loading) {
        return (
            <div className="d-flex justify-content-center py-4">
                <div className="spinner-border spinner-border-sm text-secondary" role="status">
                    <span className="visually-hidden">Đang tải danh mục...</span>
                </div>
                <span className="ms-2 text-muted small">Đang tải danh mục...</span>
            </div>
        );
    }

    return (
        <div className="d-flex flex-nowrap overflow-x-auto gap-3 pb-3 justify-content-start justify-content-md-center hide-scrollbar w-100">
            {categoryProducts.map((item) => (
                // Bọc toàn bộ card bằng Link, dẫn tới Route chi tiết danh mục
                <Link 
                    to={`/category/${item.id}`} 
                    key={item.id} 
                    className="flex-shrink-0 text-decoration-none" 
                    style={{ width: "160px" }}
                >
                    <div className="modern-category-card p-3 text-center h-100 bg-white rounded-3 border shadow-sm d-flex flex-column align-items-center justify-content-center">
                        
                        {/* Khung Icon tròn */}
                        <div 
                            className="category-icon-wrapper d-flex align-items-center justify-content-center rounded-circle mb-2"
                            style={{ width: "50px", height: "50px", backgroundColor: "#f8f9fa", color: "#495057" }}
                        >
                            <i className={`fa-solid ${getIconCategory(item.name)} fs-4`}></i>
                        </div>

                        {/* Nội dung chữ */}
                        <div className="category-text w-100">
                            <h6 className="fw-semibold text-dark mb-1 text-truncate" style={{ fontSize: "0.85rem" }}>
                                {item.name}
                            </h6>
                            <span className="text-muted d-block" style={{ fontSize: "0.75rem" }}>
                                Xem ngay <i className="fa-solid fa-chevron-right ms-1" style={{ fontSize: "0.6rem" }}></i>
                            </span>
                        </div>

                    </div>
                </Link>
            ))}
        </div>
    );
};

export default CategoryProductList;