import { DefaultButton, Dropdown, IDropdownOption, PrimaryButton, Stack, TextField } from "@fluentui/react";
import React from "react";
import { CartContext } from "../contexts/CartContext";
import { UnitOfMeasure } from "../models/UnitOfMeasure";

interface IQuantitySelectorProps {
  productName: string;
  unitOfMeasures: UnitOfMeasure[];
  onClose: () => void;
}

export const QuantitySelector = (props: IQuantitySelectorProps) => {
  const options = props.unitOfMeasures.map(uom => ({
    key: uom.name,
    text: `${uom.name} (x${uom.singlesPerUnit})`
  }));

  const cart = React.useContext(CartContext);
  const [selectedUOMIdx, setSelectedUOMIdx] = React.useState<number | undefined>();
  const [quantity, setQuantity] = React.useState(0);

  const add = () => {
    if(quantity > 0 && selectedUOMIdx && selectedUOMIdx >= 0){
      const item = {
        name: props.productName,
        unitOfMeasureName: options[selectedUOMIdx].text,
        quantity: quantity
      }
      cart.items.push(item);
      props.onClose();
    }
  };

  const handleDropdownChange = (event?: React.FormEvent<HTMLDivElement>, option?: IDropdownOption, index?: number) => {
      setSelectedUOMIdx(index);
  };

  const handleQuantityChange = React.useCallback(
    (event: React.FormEvent<HTMLInputElement | HTMLTextAreaElement>, newValue?: string) => {
      setQuantity(+(newValue ?? 0));
    },
    [],
  );

  return (
    <Stack role="document" tokens={{ childrenGap: 20 }}>
      <Stack horizontal tokens={{ childrenGap: 10 }} horizontalAlign="space-between">
        <Dropdown placeholder="Select an option" label="Unit of Measure" options={options} onChange={handleDropdownChange} />
        <TextField label="Count" type="number" min={0} styles={{ root: { width: 55 } }} onChange={handleQuantityChange}/>
      </Stack>
      <Stack horizontal tokens={{ childrenGap: 10 }} horizontalAlign="space-between">
        <PrimaryButton onClick={add} disabled={quantity === 0 || selectedUOMIdx === undefined}>Add</PrimaryButton>
        <DefaultButton onClick={props.onClose}>Cancel</DefaultButton>
      </Stack>
    </Stack>
  );
};
