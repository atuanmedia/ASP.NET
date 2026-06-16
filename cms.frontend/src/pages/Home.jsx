import React from "react";
import HeroBanner from "../components/banner/HeroBanner";
import CategoryProductList from "../components/category/CategoryProductList";
import ProductList from "../components/product/ProductList";
import HomeBlogList from "../components/post/HomeBlogList"; // Import component blog đã tối ưu Grid
import "./Home.css"; // Bắt buộc import file CSS mới này để dọn rác layout cũ

function Home() {
    return (
        <div className="modern-lp-wrapper w-100 overflow-hidden">
            {/* 1. Banner tràn viền hiện đại */}
            <HeroBanner />

            {/* 2. Khối Danh Mục Sản Phẩm (Dàn phẳng hàng ngang) */}
            <section className="modern-lp-section-category py-5 bg-light w-100">
                <div className="container">
                    <div className="row justify-content-center text-center mb-4">
                        <div className="col-12">
                            <h2 className="fw-semibold text-dark mb-2">Khám Phá Theo Danh Mục</h2>
                            <p className="text-muted small mb-0">Tìm kiếm nhanh những món đồ phù hợp cho không gian của bạn</p>
                        </div>
                    </div>
                    <CategoryProductList />
                </div>
            </section>

            {/* 3. Khối Sản Phẩm Nổi Bật (Grid 4 cột thoáng đãng) */}
            <section className="modern-lp-section-products py-5 w-100">
                <div className="container">
                    <div className="row justify-content-center text-center mb-5">
                        <div className="col-12">
                            <span className="text-uppercase tracking-wider small text-secondary d-block mb-1" style={{ letterSpacing: "2px" }}>
                                Sản phẩm nổi bật
                            </span>
                            <h2 className="fw-bold text-dark">Nội Thất Thông Minh</h2>
                            <div className="mx-auto bg-dark mt-2" style={{ width: "50px", height: "2px" }}></div>
                        </div>
                    </div>
                    <ProductList />
                </div>
            </section>

            {/* 4. Khối Tin Tức & Cẩm Nang Không Gian Sống */}
            <section className="modern-lp-section-blog py-5 bg-light w-100">
                <div className="container">
                    <div className="row justify-content-center text-center mb-5">
                        <div className="col-12">
                            <span className="text-uppercase tracking-wider small text-secondary d-block mb-1" style={{ letterSpacing: "2px" }}>
                                Cẩm nang không gian sống
                            </span>
                            <h2 className="fw-bold text-dark">Xu Hướng & Bí Quyết Mặc Đẹp</h2>
                            <div className="mx-auto bg-dark mt-2" style={{ width: "50px", height: "2px" }}></div>
                        </div>
                    </div>
                    {/* Nhúng phần danh sách blog hàng ngang vào đây */}
                    <HomeBlogList />
                </div>
            </section>
        </div>
    );
}

export default Home;