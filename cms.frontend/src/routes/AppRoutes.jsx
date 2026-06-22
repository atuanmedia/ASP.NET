import { BrowserRouter, Routes, Route } from "react-router-dom";
import { AuthProvider } from "../context/AuthContext";
import { CartProvider } from "../context/CartContext";
import MainLayout from "../layouts/MainLayout";
import Home from "../pages/Home";
import Shop from "../pages/shop/Shop";
import ProductDetail from "../pages/shop/ProductDetail";
import Cart from "../pages/Cart";
import Checkout from "../pages/Checkout";
import CategoryProductsPage from "../pages/CategoryProductsPage";
import Contact from "../pages/Contact";
import Post from "../pages/post/Post";
import PostDetail from "../pages/post/PostDetail";
import Login from "../pages/Login";
import Profile from "../pages/Profile";

function AppRoutes() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <CartProvider>
        <Routes>
          {/* Trang login — không có Header/Footer */}
          <Route path="/login" element={<Login />} />

          {/* Toàn bộ trang còn lại bọc trong MainLayout */}
          <Route element={<MainLayout />}>
            <Route path="/" element={<Home />} />
            <Route path="/shop" element={<Shop />} />
            <Route path="/product/:id" element={<ProductDetail />} />
            <Route path="/category/:id" element={<CategoryProductsPage />} />
            <Route path="/cart" element={<Cart />} />
            <Route path="/checkout" element={<Checkout />} />
            <Route path="/contact" element={<Contact />} />
            <Route path="/post" element={<Post />} />
            <Route path="/post/:id" element={<PostDetail />} />
            <Route path="/profile" element={<Profile />} />
          </Route>
        </Routes>
        </CartProvider>
      </AuthProvider>
    </BrowserRouter>
  );
}

export default AppRoutes;
