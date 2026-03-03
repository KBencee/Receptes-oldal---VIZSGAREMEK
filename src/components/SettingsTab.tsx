import ImageUpload from "./ImageUpload";

const API_BASE_URL = "https://cbnncff2-7114.euw.devtunnels.ms/api";
const token = localStorage.getItem("access");

const editUsername = (props: { username: string; reloadFunc: () => void }) => {
  fetch(`${API_BASE_URL}/Auth/me/username`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify({
      newUsername: props.username,
    }),
  })
    .then((response) => {
      if (!response.ok) {
        throw new Error("Felhasználónév módosítása sikertelen");
      }
      return response.json();
    })
    .then((data) => {
      console.log(data);
      if (data.username != localStorage.getItem("username")) {
        localStorage.setItem("username", data.username);
        alert("Profil sikeresen módosítva!");
        props.reloadFunc();
        document.getElementById("alertOverlay")!.style.display = "none";
      }
    })
    .catch((error) => {
      console.error("Felhasználónév módosítása sikertelen:", error);
    });
};

const editProfilePicture = (props: {
  image: string;
  reloadFunc: () => void;
}) => {};

const cancelButtonHandler = () => {
  const overlay = document.getElementById("alertOverlay");
  if (overlay) {
  }
};

const SettingsTab = (props: {
  imageUrl: string;
  username: string;
  reloadFunc: () => void;
}) => {
  return (
    <div className="settingsTab">
      {/* <img src={props.imageUrl} className="profileImage"/>
       */}
      <ImageUpload image={props.imageUrl} setImage={() => {}} />
      <h1 contentEditable id="usernameEdit">
        {props.username}
      </h1>
      <button
        onClick={() =>
          editUsername({
            username:
              (document.getElementById("usernameEdit") as HTMLElement)
                ?.innerText || props.username,
            reloadFunc: props.reloadFunc,
          })
        }
      >
        Mentés
      </button>
      <button onClick={cancelButtonHandler}>Mégse</button>
    </div>
  );
};

export default SettingsTab;
