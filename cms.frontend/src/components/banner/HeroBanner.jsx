import { motion } from "framer-motion";
import "./HeroBanner.css";

function HeroBanner() {
  return (
    <section className="hero-compact">
      {/* Lớp phủ mờ giúp chữ nổi bật trên nền ảnh */}
      <div className="hero-overlay-compact"></div>

      <div className="container h-100 position-relative z-1">
        <div className="row h-100 align-items-center justify-content-center text-center">
          <div className="col-12 col-md-8 col-lg-6 hero-content-compact">
            
            {/* Tiêu đề chính - Giảm khoảng cách trượt y xuống 40 để mượt và nhanh hơn */}
            <motion.h1
              initial={{ opacity: 0, y: 40 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ duration: 0.8, ease: "easeOut" }}
              className="fw-bold text-white mb-2"
            >
              Nội Thất Thông Minh
            </motion.h1>

            {/* Đoạn mô tả ngắn */}
            <motion.p
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              transition={{ delay: 0.4, duration: 0.6 }}
              className="text-white-50 mb-4 fw-light fs-6"
            >
              Không gian sống hiện đại cho ngôi nhà tương lai
            </motion.p>

            {/* Nút bấm bo tròn cao cấp */}
            <motion.button
              whileHover={{ scale: 1.05, backgroundColor: "#ffffff", color: "#000000" }}
              whileTap={{ scale: 0.95 }}
              initial={{ opacity: 0, y: 20 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ delay: 0.6 }}
              className="btn btn-outline-light rounded-pill px-4 py-2 text-uppercase tracking-wider font-semibold small-btn"
            >
              Khám Phá Ngay
            </motion.button>

          </div>
        </div>
      </div>
    </section>
  );
}

export default HeroBanner;