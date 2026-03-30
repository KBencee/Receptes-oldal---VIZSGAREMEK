
const TextUpload = ({
  ...props
}: {
  Click: () => void;
  notReadyClick: () => void;
  title: string;
  setTitle: (title: string) => void;
  description: string;
  setDescription: (description: string) => void;
  length: number;
  setLength: (length: number) => void;
  lenghtUnit: string;
  setLenghtUnit: (lenghtUnit: string) => void;
  difficulty: string;
  setDifficulty: (difficulty: string) => void;
  tags: string[];
  setTags: (tags: string[]) => void;
  ingredients: string;
  setIngredients: (ingredients: string) => void;
}) => {
  const addTag = () => {
    if (props.tags.length < 20) {
      props.setTags([...props.tags, "Cimke"]);
    }
  };
  const handleTagEdit = (index: number, newValue: string) => {
    const updatedTags = [...props.tags];
    updatedTags[index] = newValue;
    props.setTags(updatedTags);
  };

  const isPostReady = () => {
    return (
      props.title.trim() !== "" &&
      props.description.trim() !== "" &&
      props.length > 0 &&
      props.difficulty.trim() !== "" &&
      props.ingredients.trim() !== ""
    );
  }

  return (
    <div id="textContDiv">
      <form id="textCont">
        <input
          type="text"
          id="setName"
          placeholder="Add meg a recepted nevét"
          value={props.title}
          onChange={(e) => props.setTitle(e.target.value)}
          maxLength={100}
        />
        <div id="tagList">
          <div id="innerTagList">
            {props.tags.map((tag, index) => (
              <div
                key={index}
                className="tag"
                contentEditable
                suppressContentEditableWarning
                onInput={(e) => {
                  const text = e.currentTarget.textContent || "";
                  const cleanText = text.replace(/[^a-zA-Z\s]/g, "");
                  if (text !== cleanText) {
                    e.currentTarget.textContent = cleanText;
                  }
                  if (text.length > 16) {
                    e.currentTarget.textContent = cleanText.slice(0, 16);
                    const range = document.createRange();
                    const sel = window.getSelection();
                    range.selectNodeContents(e.currentTarget);
                    range.collapse(false);
                    sel?.removeAllRanges();
                    sel?.addRange(range);
                  }
                }}
                onClick={(e) => {
                  if (e.currentTarget.textContent == "Cimke") {
                    e.currentTarget.textContent = "";
                  }
                }}
                onBlur={(e) =>
                  handleTagEdit(index, e.currentTarget.textContent || "")
                }
                onKeyDown={(e) => {
                  if (e.key === "Enter") {
                    e.preventDefault();
                    e.currentTarget.blur();
                  }
                }}
                onContextMenu={(e) => {
                  e.preventDefault();
                  const updatedTags = [...props.tags];
                  updatedTags.splice(index, 1);
                  props.setTags(updatedTags);
                }}
              >
                {tag}
              </div>
            ))}
            <div
              className={props.tags.length < 10 ? "tag" : "fOff"}
              onClick={addTag}
              id="plusButton"
              onContextMenu={(e) => {
                  e.preventDefault();
                }}
            >
              +
            </div>
          </div>
        </div>
        <textarea
          id="setDescription"
          placeholder="Írd le a receptedet"
          value={props.description}
          onChange={(e) => props.setDescription(e.target.value)}
          maxLength={10000}
        ></textarea>
        <textarea
          id="setIngredients"
          placeholder="Add meg a hozzávalókat"
          value={props.ingredients}
          onChange={(e) => props.setIngredients(e.target.value)}
          maxLength={10000}
        ></textarea>
        <div id="diffLenDiv">
          <div id="diffDiv">
            <label htmlFor="diffSelect">Mennyire nehéz?</label>
            <select
              name="diffSelect"
              id="diffSelect"
              value={props.difficulty}
              onChange={(e) => props.setDifficulty(e.target.value)}
            >
              <option value="-" hidden>
                Válassz
              </option>
              <option value="Könnyű">Könnyű</option>
              <option value="Közepes">Közepes</option>
              <option value="Nehéz">Nehéz</option>
            </select>
          </div>
          <div id="lenDiv">
            <label htmlFor="lenSelect">Mennyi idő?</label>
            <div id="lenInline">
              <input
                type="text"
                id="lenSelect"
                value={props.length}
                maxLength={2}
                onChange={(e) => {
                  const value = e.target.value;
                  if (/^\d*$/.test(value)) {
                    props.setLength(Number(value) || 0);
                  }
                }}
              />

              <select
                name="lenUnitSel"
                id="lenUnitSel"
                value={props.lenghtUnit}
                onChange={(e) => props.setLenghtUnit(e.target.value)}
              >
                <option value="Min">perc</option>
                <option value="Hour">óra</option>
              </select>
            </div>
          </div>
        </div>
        <input
          type="button"
          value="Feltöltés"
          onClick={isPostReady() ? props.Click : props.notReadyClick}
          className="uploadButton"
          disabled={!isPostReady()}
        />
      </form>
    </div>
  );
};

export default TextUpload;
