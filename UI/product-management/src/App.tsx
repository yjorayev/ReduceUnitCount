import { ActionButton } from "@fluentui/react";
import { Navigate, Route, Routes, useNavigate } from "react-router-dom";
import { Cart } from "./components/Cart";
import { Products } from "./components/Products";

export const App = () => {
  const items = [
    {
      text: "Products",
      link: "/products"
    },
    {
      text: "Cart",
      link: "/cart"
    }
  ];

  const navigate = useNavigate();

  return (
    <>
        {items.map((item, index) => (
          <ActionButton key={`link${index}`} onClick={() => navigate({pathname: item.link})}>{item.text}</ActionButton>
        ))}

      <Routes>
        <Route path="/" element={<Navigate to="/products" />} />
        <Route path="/products" element={<Products />} />
        <Route path="/cart" element={<Cart />}/>
      </Routes>
    </>
  );
};
