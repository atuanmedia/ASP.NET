import { useEffect, useState } from 'react';
import productService from '../services/productService';

import ProductCard from '../components/product/ProductCard';

import './Shop.css';

function Shop() {

    const [products, setProducts] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        loadProducts();
    }, []);

    const loadProducts = async () => {

        try {

            const response =
                await productService.getShopProducts({
                    page: 1,
                    pageSize: 12
                });

            setProducts(response.products);

        } catch (error) {
            console.log(error);

        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="shop-page">

            <div className="shop-banner">

                <div className="overlay">

                    <h1>
                        Nội Thất Hiện Đại
                    </h1>

                    <p>
                        Không gian sống đẳng cấp
                        cho ngôi nhà của bạn
                    </p>

                </div>

            </div>

            <div className="container">

                <div className="shop-layout">

                    <aside className="shop-sidebar">

                        <h3>Bộ lọc</h3>

                        <input
                            type="text"
                            placeholder="Tìm sản phẩm..."
                        />

                        <div className="filter-group">

                            <h4>Khoảng giá</h4>

                            <label>
                                <input type="checkbox" />
                                Dưới 5 triệu
                            </label>

                            <label>
                                <input type="checkbox" />
                                5 - 10 triệu
                            </label>

                            <label>
                                <input type="checkbox" />
                                10 - 20 triệu
                            </label>

                            <label>
                                <input type="checkbox" />
                                Trên 20 triệu
                            </label>

                        </div>

                    </aside>

                    <section className="shop-products">

                        <div className="shop-header">

                            <h2>
                                Tất cả sản phẩm
                            </h2>

                            <select>
                                <option>
                                    Mới nhất
                                </option>

                                <option>
                                    Giá tăng dần
                                </option>

                                <option>
                                    Giá giảm dần
                                </option>
                            </select>

                        </div>

                        {loading ? (

                            <div>
                                Đang tải sản phẩm...
                            </div>

                        ) : (

                            <div className="product-grid">

                                {products.map(product => (

                                    <ProductCard
                                        key={product.id}
                                        product={product}
                                    />

                                ))}

                            </div>

                        )}

                    </section>

                </div>

            </div>

        </div>
    );
}

export default Shop;