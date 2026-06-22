import { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import categoryProductService from "../../services/categoryProductService";
import "./CategoryProductList.css";

const BACKEND = "https://localhost:7075";

const toAbsUrl = (url) => {
  if (!url) return null;
  if (url.startsWith("http")) return url;
  return BACKEND + (url.startsWith("/") ? "" : "/") + url;
};

// Màu gradient nền fallback theo index
const FALLBACK_COLORS = [
  "linear-gradient(135deg,#1a1a1a 0%,#3d3d3d 100%)",
  "linear-gradient(135deg,#2d3748 0%,#4a5568 100%)",
  "linear-gradient(135deg,#744210 0%,#b7791f 100%)",
  "linear-gradient(135deg,#1a365d 0%,#2b6cb0 100%)",
  "linear-gradient(135deg,#22543d 0%,#276749 100%)",
  "linear-gradient(135deg,#553c9a 0%,#805ad5 100%)",
];

export default function CategoryProductList() {
  const [categories, setCategories] = useState([]);
  const [loading, setLoading]       = useState(true);

  useEffect(() => {
    categoryProductService.getAllCategoryProducts()
      .then(data => setCategories(Array.isArray(data) ? data : []))
      .catch(() => {})
      .finally(() => setLoading(false));
  }, []);

  if (loading) {
    return (
      <div className="cat-skeleton-row">
        {[1,2,3,4,5,6].map(i => <div key={i} className="cat-skeleton" />)}
      </div>
    );
  }

  if (categories.length === 0) return null;

  return (
    <div className="cat-grid">
      {categories.map((cat, idx) => {
        const imgSrc = toAbsUrl(cat.imageUrl);
        const fallback = FALLBACK_COLORS[idx % FALLBACK_COLORS.length];

        return (
          <Link to={`/category/${cat.id}`} key={cat.id} className="cat-card">
            {/* Ảnh nền */}
            <div
              className="cat-card-bg"
              style={{ background: imgSrc ? undefined : fallback }}
            >
              {imgSrc && (
                <img
                  src={imgSrc}
                  alt={cat.name}
                  className="cat-card-img"
                  onError={e => { e.target.style.display = "none"; }}
                />
              )}
            </div>

            {/* Overlay gradient */}
            <div className="cat-card-overlay" />

            {/* Nội dung */}
            <div className="cat-card-content">
              <h3 className="cat-card-name">{cat.name}</h3>
              {cat.description && (
                <p className="cat-card-desc">{cat.description}</p>
              )}
              <span className="cat-card-btn">
                Xem ngay <span className="cat-arrow">→</span>
              </span>
            </div>
          </Link>
        );
      })}
    </div>
  );
}
