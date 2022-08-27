import { Stack } from "@fluentui/react";
import React from "react";
import { ProductViewModel } from "../models/ProductViewModel";
import { Product } from "./Product";

export const Products = () => {
  const [products, setProducts] = React.useState<ProductViewModel[]>([]);
  
  

  React.useEffect(() => {
    const getProducts = (): ProductViewModel[] => {
        //should call api here
        return [
          {
            name: "Chocolate",
            imageUrl: "../assets/chocolate.webp",
            unitOfMeasures: [
                {
                    name: "Pair",
                    singlesPerUnit: 2
                },
                {
                    name: "Pack",
                    singlesPerUnit: 5
                },
                {
                    name: "Dozen",
                    singlesPerUnit: 12
                }
            ]
          },
          {
            name: "Cola",
            imageUrl: "../assets/cola.jpg",
            unitOfMeasures: [
                {
                    name: "Each",
                    singlesPerUnit: 1
                },
                {
                    name: "Pack",
                    singlesPerUnit: 6
                },
                {
                    name: "Case",
                    singlesPerUnit: 24
                }
            ]
          }
        ];
      };

    setProducts(getProducts());
  }, []);

  return (
    <>
      <Stack horizontal horizontalAlign="center" tokens={{ childrenGap: 20 }}>
        {products.map((product, idx) => (
          <Product product={product} key={`product${idx}`}/>
        ))}
      </Stack>
    </>
  );
};
