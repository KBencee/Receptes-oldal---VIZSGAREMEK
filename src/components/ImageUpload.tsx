import { useEffect, useState } from "react";


const ImageUpload = ({
  ...props
}: {
  image: File | null | string
  setImage: (image: File | null) => void
  imagePath_?: string
 }) => {
  const [imagePath, setImagePath] = useState<string>(props.imagePath_ || (typeof props.image === "string" ? props.image : ""));

  useEffect(() => {
    setImagePath(props.imagePath_ || (typeof props.image === "string" ? props.image : ""));
  }, [props.image, props.imagePath_]);
  return (
    <div
      className={`imageUploadDiv ${props.image ? "has-file" : ""}`}
      onClick={() => document.getElementById("imageUpload")?.click()}
    >
      <div style={{ display: props.image ? "none" : "block" }}>
        <p>Kattints ide a kép feltöltéséhez</p>
        <input
          type="file"
          id="imageUpload"
          style={{ display: "none" }}
          accept="image/png, image/jpeg"
          onChange={(e) => {
            const files = e.target.files;
            if (files && files.length > 0) {
              const file = files[0];
              const objectUrl = URL.createObjectURL(file);
              setImagePath(objectUrl);
              if (props.setImage) props.setImage(file);
            }
          }}
        />
      </div>
      <img
        src={imagePath}
        alt="Preview"
        id="imagePreview"
        style={{ display: props.image ? "block" : "none" }}
      />
    </div>
  );
};

export default ImageUpload;
