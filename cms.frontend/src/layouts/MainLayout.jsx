import { Outlet } from "react-router-dom";
import Header from "../components/common/Header";
import Footer from "../components/common/Footer";
import "./MainLayout.css";

function MainLayout() {
  return (
    <div className="layout-container">
      {/* 1. Thanh Header luôn cố định ở trên cùng */}
      <Header />

      {/* 2. <Outlet /> thay thế cho {children}. 
           Nó sẽ tự động nhận diện URL để "bơm" các trang tương ứng vào (Home, Shop, Cart...) */}
      <main className="main-content">
        <Outlet />
      </main>

      {/* 3. Thanh Footer luôn nằm dưới cùng */}
      <Footer />
    </div>
  );
}

export default MainLayout;