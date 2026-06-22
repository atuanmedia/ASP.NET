import './Loading.css';

function Loading({ text = 'Đang tải dữ liệu...' }) {
    return (
        <div className="loading-container">
            <div className="loading-spinner"></div>
            <p>{text}</p>
        </div>
    );
}

export default Loading;