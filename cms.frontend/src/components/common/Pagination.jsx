import "./Pagination.css";

export default function Pagination({ page, totalPages, onChange }) {
  if (!totalPages || totalPages <= 1) return null;

  const items = Array.from({ length: totalPages }, (_, i) => i + 1)
    .filter(p => p === 1 || p === totalPages || Math.abs(p - page) <= 1)
    .reduce((acc, p, i, arr) => {
      if (i > 0 && p - arr[i - 1] > 1) acc.push("...");
      acc.push(p);
      return acc;
    }, []);

  return (
    <nav className="pg-wrap" aria-label="Phân trang">
      <button
        className="pg-btn"
        disabled={page === 1}
        onClick={() => onChange(page - 1)}
      >
        ← Trước
      </button>

      {items.map((item, i) =>
        item === "..." ? (
          <span key={`dot-${i}`} className="pg-dots">…</span>
        ) : (
          <button
            key={item}
            className={`pg-num ${page === item ? "active" : ""}`}
            onClick={() => onChange(item)}
          >
            {item}
          </button>
        )
      )}

      <button
        className="pg-btn"
        disabled={page === totalPages}
        onClick={() => onChange(page + 1)}
      >
        Sau →
      </button>
    </nav>
  );
}
