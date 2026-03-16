import { useState } from "react";
import ImageUpload from "../components/ImageUpload";
import TextUpload from "../components/TextUpload";
import postUpload from "../services/postUpload";
import Navbar from "../components/Navbar";
import type { ContentType } from "../types/ContentType";
import { useMobileContext } from "../context/MobileContextProvider";
import UploadAlert from "../components/UploadAlert";
import { useNavigate } from "react-router-dom";
import { menuItems, setActive } from "../components/NavMiddle";

function ContentUpload_() {
  const [title, setTitle] = useState<string>("");
  const [description, setDescription] = useState<string>("");
  const [length, setLength] = useState<number>(0);
  const [lenghtUnit, setLenghtUnit] = useState<string>("");
  const [difficulty, setDifficulty] = useState<string>("");
  const [tags, setTags] = useState<string[]>(["Cimke"]);
  const [image, setImage] = useState<File | null>(null);
  const [ingredients, setIngredients] = useState<string>("");
  const { isMobile } = useMobileContext();
  const [buttonClicked, setButtonClicked] = useState<boolean>(false);
  const [isUploadDone, setIsUploadDone] = useState<boolean>(false);

  const isPostReady = () => {
    return (
      title.trim() !== "" &&
      description.trim() !== "" &&
      length > 0 &&
      difficulty.trim() !== "" &&
      ingredients.trim() !== ""
    );
  };
  const navigate = useNavigate();
  const goToOwn = () => {
    navigate("/own");
    setActive(menuItems[1].name);
  };
  const uploadBtn_Click = async () => {
    setButtonClicked(true);
    let trueLength: number = length;
    if (lenghtUnit == "Hour") {
      trueLength = length * 60;
    }
    let trueTagList : string[] = [];
    tags.forEach((tag) => {
      if(tag.trim() !== "" && tag.trim().toLowerCase() !== "cimke"){
        trueTagList.push(tag.trim());
      }
    });
    let temp: ContentType = {
      title: title,
      description: description,
      image: image ?? "",
      length: trueLength,
      difficulty: difficulty,
      tags: trueTagList,
      ingredients: ingredients,
    };
    await postUpload(temp);
    setIsUploadDone(true);
  };

  const notReadyClick = () => {
    setButtonClicked(true);
  };

  return (
    <>
      <Navbar />
      <div id={isMobile ? "contentUploadMobile" : "contentUpload"}>
        { (
          <ImageUpload image={image} setImage={setImage}/>
        )}
        { (
          <TextUpload
            Click={uploadBtn_Click}
            notReadyClick={notReadyClick}
            title={title}
            setTitle={setTitle}
            description={description}
            setDescription={setDescription}
            length={length}
            setLength={setLength}
            lenghtUnit={lenghtUnit}
            setLenghtUnit={setLenghtUnit}
            difficulty={difficulty}
            setDifficulty={setDifficulty}
            tags={tags}
            setTags={setTags}
            ingredients={ingredients}
            setIngredients={setIngredients}
          />
        )}
        {isUploadDone && (
          <>
            <div id="alertOverlay" />
            <UploadAlert
              text="Recepted feltöltése sikeres!"
              onClick={goToOwn}
            />
          </>
        )}
        {buttonClicked && !isPostReady() && (
          <>
            <div id="alertOverlay" />
            <UploadAlert
              text="Töltsd ki az összes mezőt a feltöltéshez!"
              onClick={() => setButtonClicked(false)}
            />
          </>
        )}
      </div>
    </>
  );
}

export default ContentUpload_;
