import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom"; // 🌟 1. Import useNavigate
import productService from "../../services/productService";
import "./ProductList.css"; 

const ProductList = () => {
    const [products, setProducts] = useState([]);
    const [loading, setLoading] = useState(true);
    const navigate = useNavigate(); // 🌟 2. Khởi tạo hàm điều hướng

    useEffect(() => {
        const fetchProducts = async () => {
            try {
                const response = await productService.getAllProducts();
                setProducts(response.data || response);
            } catch (error) {
                console.error("Lỗi khi tải danh sách sản phẩm:", error);
            } finally {
                setLoading(false);
            }
        };

        fetchProducts();
    }, []);

    // 🌟 3. Hàm xử lý chuyển hướng khi click xem chi tiết
    const handleViewDetail = (productId) => {
        navigate(`/product/${productId}`);
    };

    // 🌟 4. Hàm xử lý đường dẫn ảnh chuẩn đét (Tránh lỗi ERR_EMPTY_RESPONSE)
    const getProductImage = (imageUrl) => {
        const DEFAULT_IMAGE = "https://placehold.co/400x400?text=No+Image";
        if (!imageUrl) return DEFAULT_IMAGE;
        if (imageUrl.startsWith("http://") || imageUrl.startsWith("https://")) {
            return imageUrl;
        }
        const BACKEND_URL = "https://localhost:7075"; 
        return `${BACKEND_URL}${imageUrl.startsWith("/") ? "" : "/"}${imageUrl}`;
    };

    // Giao diện Đợi tải dữ liệu (Skeleton) phong cách cao cấp
    if (loading) {
        return (
            <div className="row g-4">
                {[1, 2, 3, 4].map((n) => (
                    <div className="col-12 col-sm-6 col-md-4 col-lg-3" key={n}>
                        <div className="card border-0 p-3 placeholder-glow" style={{ height: "380px", borderRadius: "4px" }}>
                            <div className="placeholder bg-secondary opacity-10 w-100 h-50 mb-4"></div>
                            <div className="placeholder bg-secondary opacity-10 w-75 mb-2" style={{ height: "15px" }}></div>
                            <div className="placeholder bg-secondary opacity-10 w-50 mb-4" style={{ height: "12px" }}></div>
                            <div className="placeholder bg-secondary opacity-10 w-100" style={{ height: "35px" }}></div>
                        </div>
                    </div>
                ))}
            </div>
        );
    }

    if (products.length === 0) {
        return <p className="text-center text-muted my-5">Bộ sưu tập nội thất hiện đang được cập nhật.</p>;
    }

    return (
        <div className="row g-4">
            {products
                .filter(product => product !== null && product !== undefined) // Phòng ngự chống sập trang
                .map((product) => {
                    const displayImage = getProductImage(product.imageUrl);

                    return (
                        <div className="col-12 col-sm-6 col-md-4 col-lg-3" key={product.id}>
                            <div className="furniture-card shadow-sm h-100 d-flex flex-column justify-content-between">
                                
                                {/* Nhãn định vị thương hiệu nội thất */}
                                <div className="furniture-badge">Collection</div>

                                {/* 1. Khung chứa ảnh sản phẩm & Sự kiện Click xem chi tiết */}
                                <div 
                                    className="furniture-img-container p-4 d-flex align-items-center justify-content-center position-relative" 
                                    style={{ height: "240px", cursor: "pointer" }}
                                    onClick={() => handleViewDetail(product.id)} // Click vào ảnh để xem chi tiết
                                >
                                    <img 
                                        src={displayImage} 
                                        className="img-fluid object-fit-contain h-100 w-100" 
                                        alt={product.name} 
                                        onError={(e) => { 
                                            e.target.onerror = null; 
                                            e.target.src = "https://placehold.co/400x400?text=No+Image"; 
                                        }}
                                    />
                                </div>

                                {/* 2. Phần thông tin chi tiết */}
                                <div className="p-4 text-center d-flex flex-column flex-grow-1 justify-content-between">
                                    <div>
                                        {/* Tên sản phẩm - Click để xem chi tiết */}
                                        <h5 
                                            className="furniture-title fw-medium text-truncate mb-2" 
                                            title={product.name}
                                            style={{ cursor: "pointer" }}
                                            onClick={() => handleViewDetail(product.id)} 
                                        >
                                            {product.name}
                                        </h5>
                                        
                                        {/* Mô tả sản phẩm */}
                                        <p className="text-muted small text-truncate mb-3" style={{ fontSize: "0.78rem", fontStyle: "italic" }}>
                                            {product.description || "Chất liệu cao cấp, gia công tinh xảo"}
                                        </p>
                                    </div>

                                    <div>
                                        {/* Giá tiền nội thất đỏ trầm quý phái */}
                                        <p className="fw-bold mb-3" style={{ color: "#8a1c1c", fontSize: "1.05rem", letterSpacing: "0.5px" }}>
                                            {typeof product.price === 'number'
                                                ? product.price.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })
                                                : `${product.price}`}
                                        </p>

                                        {/* 🌟 5. NÚT XEM CHI TIẾT (Thay cho nút Khám phá cũ) */}
                                        <button 
                                            className="btn btn-furniture w-100"
                                            onClick={() => handleViewDetail(product.id)}
                                        >
                                            Xem chi tiết
                                        </button>
                                    </div>
                                </div>

                            </div>
                        </div>
                    );
                })}
        </div>
    );
};

export default ProductList;