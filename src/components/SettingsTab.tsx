import { use, useEffect } from "react";
import ImageUpload from "./ImageUpload";
import { toast } from "sonner";
import { useQueryClient, type QueryClient } from "@tanstack/react-query";

const API_BASE_URL = "https://cbnncff2-7114.euw.devtunnels.ms/api";
const token = localStorage.getItem("access");

const editUsername = (props: {
  username: string;
  reloadFunc: () => void;
  queryClient: QueryClient;
}) => {
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
        toast("Profil sikeresen módosítva!");
        // props.reloadFunc();
        props.queryClient.invalidateQueries({ queryKey: ["me"] });
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
  onClick: () => void;
}) => {
  const queryClient = useQueryClient();

  useEffect(() => {
    console.log(props.imageUrl);
  }, [props.imageUrl]);

  return (
    <div className="settingsTab">
      {/* <img src={props.imageUrl} className="profileImage"/>
       */}
      <ImageUpload image={props.imageUrl} setImage={() => {}} />
      <h1 contentEditable id="usernameEdit">
        {props.username}
      </h1>
      <h3>(Kattints a változtatáshoz)</h3>
      <button
        onClick={() =>
          editUsername({
            username:
              (document.getElementById("usernameEdit") as HTMLElement)
                ?.innerText || props.username,
            reloadFunc: props.reloadFunc,
            queryClient: queryClient,
          })
        }
      >
        Mentés
      </button>
      <button onClick={props.onClick}>Mégse</button>
    </div>
  );
};

export default SettingsTab;
