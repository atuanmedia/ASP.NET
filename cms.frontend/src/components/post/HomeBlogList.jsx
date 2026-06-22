import React, { useState, useEffect } from "react";
import blogService from "../../services/blogService";

const HomeBlogList = () => {
  const [posts, setPosts] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchLatestPosts = async () => {
      try {
        setLoading(true);
        const response = await blogService.getAllPosts();
        
        // Khớp cấu trúc: lấy mảng trực tiếp hoặc từ response.data
        const allPosts = response.data || response;
        
        // Lấy đúng 3 bài viết mới nhất để dàn hàng ngang 3 cột tuyệt đẹp trên trang chủ
        setPosts(Array.isArray(allPosts) ? allPosts.slice(0, 3) : []);
      } catch (error) {
        console.error("Quá trình kết nối API bài viết thất bại:", error);
      } finally {
        setLoading(false);
      }
    };

    fetchLatestPosts();
  }, []);

  // Giao diện chờ (Loading Skeleton) đồng bộ phong cách mượt mà của hệ thống
  if (loading) {
    return (
      <div className="row g-4">
        {[1, 2, 3].map((n) => (
          <div className="col-12 col-md-4" key={n}>
            <div className="card border-0 shadow-sm placeholder-glow" style={{ height: "350px" }}>
              <div className="placeholder bg-secondary opacity-10 w-100 h-50 rounded-3 mb-3"></div>
              <div className="placeholder bg-secondary opacity-10 w-25 mb-2 ms-3"></div>
              <div className="placeholder bg-secondary opacity-10 w-75 mb-2 ms-3"></div>
              <div className="placeholder bg-secondary opacity-10 w-50 ms-3"></div>
            </div>
          </div>
        ))}
      </div>
    );
  }

  // Nếu không tìm thấy bài viết nào trong Database
  if (posts.length === 0) {
    return (
      <div className="alert alert-light text-center border py-4">
        <p className="text-muted m-0">Hiện tại chưa có bài viết xu hướng nào trong hệ thống.</p>
      </div>
    );
  }

  return (
    /* Dùng hệ thống lưới Bootstrap 5: g-4 tạo khoảng cách, trên PC chia thành 3 cột (col-md-4) nằm ngang */
    <div className="row g-4">
      {posts.map((item) => (
        <div className="col-12 col-md-4" key={item.id}>
          <div className="modern-blog-card bg-white rounded-3 overflow-hidden shadow-sm h-100 d-flex flex-column border">
            
            {/* 1. Khung chứa ảnh Cover - Đưa ảnh Unsplash nội thất cao cấp vào làm mặc định nếu thiếu ảnh */}
            <div className="blog-img-wrapper position-relative overflow-hidden" style={{ height: "200px" }}>
              <img 
                src={item.imageUrl  || "https://images.unsplash.com/photo-1513694203232-719a280e022f?q=80&w=600"} 
                alt={item.title}
                className="w-100 h-100 object-fit-cover"
                onError={(e) => { e.target.src = "https://images.unsplash.com/photo-1513694203232-719a280e022f?q=80&w=600" }}
              />
            </div>

            {/* 2. Phần nội dung chi tiết bài viết */}
            <div className="p-3 flex-grow-1 d-flex flex-column justify-content-between">
              <div>
                {/* Ngày tháng đăng bài (Đổi từ mr-1 cũ sang me-1 chuẩn Bootstrap 5) */}
                <div className="text-muted d-flex align-items-center mb-2" style={{ fontSize: "0.75rem" }}>
                  <i className="fa-regular fa-calendar-days me-1 text-secondary"></i>
                  {item.createdDate ? new Date(item.createdDate).toLocaleDateString('vi-VN') : "14/06/2026"}
                </div>
                
                {/* Tiêu đề bài viết - Giới hạn tối đa 2 dòng không lo vỡ hàng */}
                <h5 className="fs-6 fw-bold text-dark text-truncate-2 mb-2" title={item.title}>
                  <a href={`/post/${item.id}`} className="text-dark text-decoration-none text-hover-primary">
                    {item.title}
                  </a>
                </h5>
                
                {/* Mô tả ngắn trích dẫn (Fix lỗi thuộc tính shortDescription của bạn) */}
                <p className="text-muted text-truncate-2 mb-3" style={{ fontSize: "0.8rem", lineHeight: "1.4" }}>
                  {item.shortDescription || 'Nhấn để xem chi tiết bài viết chia sẻ về xu hướng phối đồ và bài trí không gian...'}
                </p>
              </div>

              {/* 3. Thanh tương tác dưới cùng - Thay thế badge-pill cũ bằng rounded-pill tinh tế */}
              <div className="d-flex justify-content-between align-items-center pt-2 border-top border-light">
                <a href={`/post/${item.id}`} className="text-dark fw-semibold text-decoration-none small d-inline-flex align-items-center read-more-link">
                  Đọc thêm <i className="fa-solid fa-angle-right ms-1" style={{ fontSize: "0.7rem" }}></i>
                </a>
                <span className="badge rounded-pill bg-light text-dark border px-3 py-2 cursor-pointer small-tag">
                  Tin tức
                </span>
              </div>
            </div>

          </div>
        </div>
      ))}
    </div>
  );
};

export default HomeBlogList;