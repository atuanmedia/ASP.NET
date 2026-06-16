import React, { useEffect, useState } from "react";
import { useParams, Link } from "react-router-dom"; 
import { FiArrowLeft, FiGrid, FiArrowRight, FiInbox } from "react-icons/fi"; 
import categoryProductService from "../services/categoryProductService";
import ProductCard from "../components/product/ProductCard"; 
import "./CategoryProductsPage.css";

const CategoryProductsPage = () => {
    const { id } = useParams(); 
    const [categoryData, setCategoryData] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchProductsByCategory = async () => {
            setLoading(true);
            try {
                const data = await categoryProductService.getCategoryByIdWithProducts(id);
                console.log("Dữ liệu sản phẩm theo danh mục thực tế:", data.products);
                setCategoryData(data);
            } catch (error) {
                console.error("Lỗi khi tải sản phẩm theo danh mục:", error);
            } finally {
                setLoading(false);
            }
        };

        if (id) {
            fetchProductsByCategory();
        }
    }, [id]);

    if (loading) {
        return (
            <div className="at-loading-wrapper">
                <div className="at-minimal-spinner"></div>
                <p>Khơi nguồn không gian sống...</p>
            </div>
        );
    }

    if (!categoryData) {
        return (
            <div className="container">
                <div className="at-error-container text-center py-5">
                    <h3>Bộ sưu tập không tồn tại hoặc đã được dời đi.</h3>
                    <Link to="/shop" className="at-btn-outline mt-3">
                        <FiArrowLeft /> Quay lại cửa hàng
                    </Link>
                </div>
            </div>
        );
    }

    return (
        <div className="at-category-page">
            <div className="container">
                
                {/* HERO BANNER - ĐẬM VIBE KIẾN TRÚC */}
                <div className="at-category-hero">
                    <div className="at-hero-content">
                        <div className="at-breadcrumb-minimal">
                            <Link to="/">AT Design</Link>
                            <span className="sep">/</span>
                            <Link to="/shop">Collections</Link>
                            <span className="sep">/</span>
                            <span className="current">{categoryData.name}</span>
                        </div>
                        <h1 className="at-page-title">{categoryData.name}</h1>
                        {categoryData.description && <p className="at-page-subtitle">{categoryData.description}</p>}
                    </div>
                </div>

                {/* TOOLBAR LỌC & ĐẾM */}
                <div className="at-filter-toolbar">
                    <div className="toolbar-left">
                        <FiGrid className="icon-grid" />
                        <span className="count-text">
                            Hiển thị <strong>{categoryData.products?.length || 0}</strong> tuyệt tác thiết kế
                        </span>
                    </div>
                    <div className="toolbar-right">
                        <span className="filter-label">Studio Mode</span>
                    </div>
                </div>

                {/* LUỒNG HIỂN THỊ SẢN PHẨM */}
                {categoryData.products && categoryData.products.length > 0 ? (
                    
                    <div className="at-products-grid">
                        {categoryData.products.map((product) => (
                            <ProductCard 
                                key={product.id} 
                                product={{
                                    ...product,
                                    categoryProductName: categoryData.name 
                                }} 
                            />
                        ))}
                    </div>

                ) : (
                    /* TRẠNG THÁI TRỐNG LUXURY VIBE */
                    <div className="at-empty-studio">
                        <FiInbox className="empty-studio-icon" />
                        <h2>Bộ sưu tập đang được chế tác</h2>
                        <p>Những thiết kế mới nhất cho danh mục này đang nằm trong phòng lab thử nghiệm của AT Design và sẽ sớm ra mắt.</p>
                        <Link to="/shop" className="at-btn-dark">
                            Khám phá các BST khác <FiArrowRight />
                        </Link>
                    </div>
                )}

            </div>
        </div>
    );
};

export default CategoryProductsPage;