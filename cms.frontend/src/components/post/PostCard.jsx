import { Link } from "react-router-dom";
import "./PostCard.css";

const BACKEND = "https://localhost:7075";
const FALLBACK = "https://images.unsplash.com/photo-1555041469-a586c61ea9bc?w=800&q=80";

const toAbsUrl = (url) => {
  if (!url) return FALLBACK;
  if (url.startsWith("http")) return url;
  return BACKEND + (url.startsWith("/") ? "" : "/") + url;
};

const fmtDate = (d) =>
  new Date(d).toLocaleDateString("vi-VN", { day: "2-digit", month: "short", year: "numeric" });

const readTime = (text) => {
  const words = (text || "").replace(/<[^>]+>/g, "").split(/\s+/).filter(Boolean).length;
  return Math.max(1, Math.round(words / 200));
};

export default function PostCard({ post, featured = false }) {
  if (!post) return null;

  const imgSrc = toAbsUrl(post.imageUrl);
  const mins   = readTime(post.shortDescription || "");

  return (
    <article className={`pn-card ${featured ? "pn-card--featured" : ""}`}>
      {/* Image */}
      <Link to={`/post/${post.id}`} className="pn-card-img-wrap">
        <img
          src={imgSrc}
          alt={post.title}
          className="pn-card-img"
          onError={e => { e.target.src = FALLBACK; }}
        />
        {post.categoryName && (
          <span className="pn-card-category">{post.categoryName}</span>
        )}
      </Link>

      {/* Body */}
      <div className="pn-card-body">
        {/* Meta */}
        <div className="pn-card-meta">
          <time className="pn-card-date">{fmtDate(post.createdDate)}</time>
          <span className="pn-card-dot" />
          <span className="pn-card-read">{mins} phút đọc</span>
        </div>

        {/* Title */}
        <h2 className="pn-card-title">
          <Link to={`/post/${post.id}`}>{post.title}</Link>
        </h2>

        {/* Excerpt */}
        {post.shortDescription && (
          <p className="pn-card-desc">{post.shortDescription}</p>
        )}

        {/* CTA */}
        <Link to={`/post/${post.id}`} className="pn-card-cta">
          Đọc bài viết <span className="pn-card-cta-arrow">→</span>
        </Link>
      </div>
    </article>
  );
}
