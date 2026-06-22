import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useCart } from "../../context/CartContext";
import "./ProductCard.css";

const DEFAULT_IMAGE = "https://images.unsplash.com/photo-1505693416388-ac5ce068fe85";

const ProductCard = ({ product }) => {
    const navigate = useNavigate();
    const { addItem } = useCart();
    const [added, setAdded] = useState(false);

    const handleAddToCart = (e) => {
        e.stopPropagation();
        addItem(product, 1);
        setAdded(true);
        setTimeout(() => setAdded(false), 1800);
    };

    const handleViewDetail = () => {
        navigate(`/product/${product.id}`);
    };

   const getProductImage = () => {
        // 1. Lấy ra chuỗi đường dẫn ảnh thô từ dữ liệu
        let imgPath = product.imageUrl || 
                      product.image || 
                      (typeof product.images?.[0] === "string" ? product.images[0] : product.images?.[0]?.url) || 
                      DEFAULT_IMAGE;

        // Nếu rơi vào ảnh mặc định Unsplash thì trả về luôn
        if (imgPath === DEFAULT_IMAGE) return DEFAULT_IMAGE;

        // 2. Nếu đường dẫn đã là link đầy đủ (bắt đầu bằng http:// hoặc https://) thì giữ nguyên
        if (imgPath && (imgPath.startsWith("http://") || imgPath.startsWith("https://"))) {
            return imgPath;
        }

        // 3. Nếu là đường dẫn tương đối từ server (như /uploads/...), tự động nối thêm Domain Backend của bạn
        // 🌟 Chú ý: Thay thế "http://localhost:5000" bằng đúng địa chỉ IP/Port của Server Backend bạn đang chạy thực tế
        const BACKEND_URL = "https://localhost:7075"; 
        
        return `${BACKEND_URL}${imgPath.startsWith("/") ? "" : "/"}${imgPath}`;
    };

    const displayImage = getProductImage();

    return (
        <div className="furniture-card h-100">
            <div className="furniture-badge">Collection</div>

            <div className="furniture-img-container" onClick={handleViewDetail} style={{ cursor: "pointer" }}>
                <img
                    src={displayImage}
                    alt={product.name}
                    className="furniture-image"
                    onError={(e) => {
                        e.target.onerror = null;
                        e.target.src = DEFAULT_IMAGE;
                    }}
                />
            </div>

            <div className="furniture-content">
                <span className="furniture-category">
                    {product.categoryProductName || "Nội thất"}
                </span>

                <h3 className="furniture-title" title={product.name} onClick={handleViewDetail} style={{ cursor: "pointer" }}>
                    {product.name}
                </h3>

                <p className="furniture-description">
                    {product.description || "Thiết kế tinh tế, chất liệu cao cấp."}
                </p>

                <div className="furniture-footer">
                    <div className="furniture-price">
                        {typeof product.price === "number"
                            ? product.price.toLocaleString("vi-VN", { style: "currency", currency: "VND" })
                            : product.price}
                    </div>

                    <div style={{ display: "flex", gap: "8px" }}>
                        <button className="btn-furniture" onClick={handleViewDetail}>
                            Xem chi tiết
                        </button>
                        <button
                            className="btn-furniture btn-add-cart"
                            onClick={handleAddToCart}
                            title="Thêm vào giỏ hàng"
                        >
                            {added ? "✓" : "🛒"}
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default ProductCard;