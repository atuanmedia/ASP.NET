import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { FiSearch, FiShoppingCart, FiUser, FiHeart, FiMenu, FiLogOut, FiChevronDown } from "react-icons/fi";
import { useAuth } from "../../context/AuthContext";
import { useCart } from "../../context/CartContext";
import "./Header.css";

function Header() {
  const [searchQuery, setSearchQuery] = useState("");
  const [showUserMenu, setShowUserMenu] = useState(false);
  const navigate = useNavigate();
  const { user, logout, isLoggedIn } = useAuth();
  const { totalItems } = useCart();

  const handleSearchSubmit = (e) => {
    e.preventDefault();
    if (searchQuery.trim()) {
      navigate(`/shop?search=${searchQuery}`);
    }
  };

  const handleLogout = () => {
    logout();
    setShowUserMenu(false);
    navigate("/");
  };

  return (
    <header className="at-header">
      <div className="at-header-container">

        {/* 1. LOGO */}
        <div className="at-logo">
          <Link to="/">
            AT<span>Design</span>
          </Link>
        </div>

        {/* 2. SEARCH BAR */}
        <form className="at-search-bar" onSubmit={handleSearchSubmit}>
          <input
            type="text"
            placeholder="Tìm kiếm nội thất hiện đại..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
          />
          <button type="submit" aria-label="Search">
            <FiSearch className="search-icon" />
          </button>
        </form>

        {/* 3. NAV MENU */}
        <nav className="at-nav-menu">
          <Link to="/">Trang chủ</Link>
          <Link to="/shop">Sản phẩm</Link>
          <Link to="/post">Tin Tức</Link>
          <Link to="/contact">Liên hệ</Link>
        </nav>

        {/* 4. ACTION ICONS */}
        <div className="at-header-actions">
          <Link to="/wishlist" className="action-item" title="Yêu thích">
            <FiHeart />
            <span className="action-badge">0</span>
          </Link>

          <Link to="/cart" className="action-item" title="Giỏ hàng">
            <FiShoppingCart />
            {totalItems > 0 && <span className="action-badge cart-count">{totalItems}</span>}
          </Link>

          {/* Tài khoản — thay đổi theo trạng thái đăng nhập */}
          {isLoggedIn ? (
            <div className="user-menu-wrap">
              <button
                className="user-menu-trigger"
                onClick={() => setShowUserMenu((v) => !v)}
                title="Tài khoản"
              >
                <div className="user-avatar">
                  {user.fullName?.charAt(0).toUpperCase() || user.email?.charAt(0).toUpperCase()}
                </div>
                <span className="user-name">{user.fullName || user.email}</span>
                <FiChevronDown className={`user-chevron ${showUserMenu ? "open" : ""}`} />
              </button>

              {showUserMenu && (
                <div className="user-dropdown">
                  <div className="user-dropdown-info">
                    <p className="user-dropdown-name">{user.fullName}</p>
                    <p className="user-dropdown-role">{user.role}</p>
                  </div>
                  <hr className="user-dropdown-divider" />

    {/* 👉 LINK PROFILE */}
    <Link
      to="/profile"
      className="user-dropdown-item"
      onClick={() => setShowUserMenu(false)}
    >
      <FiUser style={{ marginRight: 8 }} />
      Hồ sơ cá nhân
    </Link>
                  <button className="user-dropdown-logout" onClick={handleLogout}>
                    <FiLogOut style={{ marginRight: 8 }} />
                    Đăng xuất
                  </button>
                </div>
              )}
            </div>
          ) : (
            <Link to="/login" className="action-item account-link" title="Đăng nhập">
              <FiUser />
              <span className="account-text">Đăng nhập</span>
            </Link>
          )}

          <button className="mobile-menu-toggle" aria-label="Toggle Menu">
            <FiMenu />
          </button>
        </div>

      </div>
    </header>
  );
}

export default Header;
