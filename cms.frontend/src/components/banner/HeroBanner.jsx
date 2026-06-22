import { useEffect, useState, useRef, useCallback } from "react";
import { Link } from "react-router-dom";
import advertisementService from "../../services/advertisementService";
import "./HeroBanner.css";

// Slide mặc định hiển thị khi API chưa load hoặc rỗng
const FALLBACK = [
  {
    id: 0,
    title: "Nội Thất Thông Minh",
    subtitle: "Không gian sống hiện đại cho ngôi nhà tương lai",
    buttonText: "Khám phá ngay",
    buttonLink: "/shop",
    imageUrl: "https://images.unsplash.com/photo-1555041469-a586c61ea9bc?w=1600&q=80",
  },
];

export default function HeroBanner() {
  const [slides, setSlides]       = useState(FALLBACK);
  const [current, setCurrent]     = useState(0);
  const [animating, setAnimating] = useState(false);
  const [paused, setPaused]       = useState(false);
  const timerRef = useRef(null);

  // Tải danh sách slide từ API
  useEffect(() => {
    advertisementService.getActive().then(data => {
      if (Array.isArray(data) && data.length > 0) setSlides(data);
    }).catch(() => {});
  }, []);

  const goTo = useCallback((idx) => {
    if (animating) return;
    setAnimating(true);
    setCurrent(idx);
    setTimeout(() => setAnimating(false), 700);
  }, [animating]);

  const next = useCallback(() => {
    goTo((current + 1) % slides.length);
  }, [current, slides.length, goTo]);

  const prev = useCallback(() => {
    goTo((current - 1 + slides.length) % slides.length);
  }, [current, slides.length, goTo]);

  // Auto-play mỗi 5 giây
  useEffect(() => {
    if (slides.length <= 1 || paused) return;
    timerRef.current = setTimeout(next, 5000);
    return () => clearTimeout(timerRef.current);
  }, [current, paused, slides.length, next]);

  const slide = slides[current];

  return (
    <section
      className="hero-slider"
      onMouseEnter={() => setPaused(true)}
      onMouseLeave={() => setPaused(false)}
    >
      {/* Background slides */}
      {slides.map((s, i) => (
        <div
          key={s.id}
          className={`hero-slide ${i === current ? "active" : ""}`}
          style={{ backgroundImage: `url(${s.imageUrl})` }}
          aria-hidden={i !== current}
        />
      ))}

      {/* Overlay */}
      <div className="hero-overlay" />

      {/* Content */}
      <div className="hero-content">
        <p className="hero-eyebrow">ATDesign Collection</p>
        <h1 className="hero-title" key={`title-${current}`}>
          {slide.title}
        </h1>
        {slide.subtitle && (
          <p className="hero-subtitle" key={`sub-${current}`}>
            {slide.subtitle}
          </p>
        )}
        {slide.buttonText && (
          <Link
            to={slide.buttonLink || "/shop"}
            className="hero-btn"
            key={`btn-${current}`}
          >
            {slide.buttonText}
            <span className="hero-btn-arrow">→</span>
          </Link>
        )}
      </div>

      {/* Arrows */}
      {slides.length > 1 && (
        <>
          <button className="hero-arrow hero-arrow--left" onClick={prev} aria-label="Slide trước">
            ‹
          </button>
          <button className="hero-arrow hero-arrow--right" onClick={next} aria-label="Slide tiếp">
            ›
          </button>
        </>
      )}

      {/* Dots */}
      {slides.length > 1 && (
        <div className="hero-dots">
          {slides.map((_, i) => (
            <button
              key={i}
              className={`hero-dot ${i === current ? "active" : ""}`}
              onClick={() => goTo(i)}
              aria-label={`Slide ${i + 1}`}
            />
          ))}
        </div>
      )}

      {/* Progress bar */}
      {slides.length > 1 && !paused && (
        <div className="hero-progress" key={`prog-${current}`} />
      )}
    </section>
  );
}
