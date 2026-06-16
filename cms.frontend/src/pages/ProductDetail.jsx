import { useEffect, useState } from "react";
import { useParams, Link } from "react-router-dom";
import productService from "../services/productService";
import AOS from "aos";
import "aos/dist/aos.css";
import "./ProductDetail.css";

function ProductDetail() {
    const { id } = useParams();

    const [product, setProduct] = useState(null);
    const [loading, setLoading] = useState(true);
    const [quantity, setQuantity] = useState(1);

    useEffect(() => {
        AOS.init({
            duration: 1000,
            once: true,
            offset: 100,
            easing: "ease-in-out",
        });

        loadProductDetail();

        window.scrollTo({
            top: 0,
            behavior: "smooth",
        });
    }, [id]);

    const loadProductDetail = async () => {
        try {
            setLoading(true);

            const response = await productService.getProductById(id);
            const productData = response.data || response;

            setProduct(productData);

            setTimeout(() => {
                AOS.refresh();
            }, 100);
        } catch (error) {
            console.error("Lỗi khi tải chi tiết sản phẩm:", error);
        } finally {
            setLoading(false);
        }
    };

    const handleQuantityChange = (type) => {
        if (type === "decrease" && quantity > 1) {
            setQuantity(quantity - 1);
        }

        if (type === "increase") {
            setQuantity(quantity + 1);
        }
    };

    if (loading) {
        return (
            <div className="detail-loading">
                <div className="spinner"></div>
                <p>Đang tải thông tin sản phẩm...</p>
            </div>
        );
    }

    if (!product) {
        return (
            <div className="detail-error">
                <h2>Không tìm thấy sản phẩm!</h2>

                <Link to="/shop" className="btn-back">
                    Quay lại cửa hàng
                </Link>
            </div>
        );
    }

return (
    <div className="product-detail-page">
        <div className="container">

            {/* 1. THANH ĐIỀU HƯỚNG (BREADCRUMB) */}
            <div className="breadcrumb">
                <a href="/">Trang chủ</a>
                <span>/</span>
                <a href="/products">Sản phẩm</a>
                <span>/</span>
                <strong>{product.name}</strong>
            </div>

            {/* 2. KHỐI TRÊN: CHIA 2 CỘT (ẢNH & THÔNG TIN MUA HÀNG) */}
            <div className="detail-top-block">
                
                {/* CỘT TRÁI: HÌNH ẢNH SẢN PHẨM */}
                <div className="detail-left-image">
                    <div className="image-wrapper">
                        <img 
                            src={product.imageUrl || "https://images.unsplash.com/photo-1505693416388-ac5ce068fe85"} 
                            alt={product.name} 
                        />
                    </div>
                </div>

                {/* CỘT PHẢI: CHI TIẾT GIÁ & NÚT ĐẶT HÀNG */}
                <div className="detail-right-info">
                    <span className="brand-badge">SMART FURNITURE</span>
                    <h1 className="product-title">{product.name}</h1>
                    
                    <div className="rating-status">
                        <span className="stars">⭐⭐⭐⭐⭐</span>
                        <span className="reviews">(25 đánh giá)</span>
                        <span className="divider">|</span>
                        <span className="stock">Tình trạng: <span className="in-stock">Còn hàng</span></span>
                    </div>

                    <div className="price-container">
                        <span className="current-price">
                            {typeof product.price === "number"
                                ? product.price.toLocaleString("vi-VN", { style: "currency", currency: "VND" })
                                : product.price}
                        </span>
                    </div>

                    <p className="short-desc">
                        {product.shortDescription || "Thiết kế tinh tế, chất liệu cao cấp mang lại không gian sống hiện đại và đẳng cấp cho ngôi nhà của bạn."}
                    </p>

                    {/* Khối tiện ích nhỏ */}
                    <div className="utility-grid">
                        <div className="utility-item">✓ Thiết kế hiện đại</div>
                        <div className="utility-item">✓ Chất liệu cao cấp</div>
                        <div className="utility-item">✓ Bảo hành chính hãng</div>
                        <div className="utility-item">✓ Giao hàng toàn quốc</div>
                    </div>

                    {/* Khu vực chọn số lượng và nút mua */}
                    <div className="action-row">
                        <div className="quantity-box">
                            <button onClick={() => handleQuantityChange("decrease")}>−</button>
                            <input type="number" value={quantity} readOnly />
                            <button onClick={() => handleQuantityChange("increase")}>+</button>
                        </div>
                        <button className="add-to-cart-btn">
                            Thêm vào giỏ hàng
                        </button>
                    </div>

                    <div className="meta-info">
                        <p><strong>Mã sản phẩm:</strong> SKU-{product.id || "09"}</p>
                        <p><strong>Danh mục:</strong> Nội thất thông minh</p>
                    </div>
                </div>

            </div> {/* KẾT THÚC KHỐI TRÊN */}


            {/* 3. KHỐI DƯỚI: MÔ TẢ CHI TIẾT SẢN PHẨM (FULL WIDTH) */}
            <div className="product-tabs-section">
                <div className="tab-title">
                    <h2>Mô tả chi tiết sản phẩm</h2>
                </div>
                <div className="tab-body">
                    <p>{product.description || "Tựa lưng có thể tháo rời nên có thể mang vào khi lối vào có chiều rộng từ 55 cm trở lên. Vải ấm áp và thích hợp để phối hợp. Hướng dẫn chăm sóc: Sử dụng máy hút bụi để loại bỏ bụi bẩn hoặc vết ố hoặc bàn chải mềm. Đối với những vết bẩn cứng đầu, hãy nhẹ nhàng vắt một miếng vải ngâm trong dung dịch tẩy rửa trung tính pha loãng và gõ nhẹ để loại bỏ vết bẩn. Sau đó, lau kỹ bằng vải vắt kiệt nước để đảm bảo không còn chất tẩy rửa."}</p>
                </div>
            </div>

            {/* 4. KHỐI DƯỚI CÙNG: SẢN PHẨM LIÊN QUAN (FULL WIDTH) */}
            <div className="related-products-section">
                <div className="section-title">
                    <h2>Sản phẩm liên quan</h2>
                </div>
                <div className="related-box-placeholder">
                    <p>Các sản phẩm liên quan sẽ hiển thị tại đây.</p>
                </div>
            </div>

        </div>
    </div>
);
}

export default ProductDetail;