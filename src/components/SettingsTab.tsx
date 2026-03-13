import { useEffect } from "react";
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
        console.log("Username updated to:", data.username);
        props.queryClient.invalidateQueries({ queryKey: ["me"] });
      }
    })
    .catch((error) => {
      console.error("Felhasználónév módosítása sikertelen:", error);
    });
};

const editProfilePicture = (props: {
  image: File | string;
  reloadFunc: () => void;
  queryClient: QueryClient;
}) => {
  const formData = new FormData();
  fetch(
    typeof props.image === "string"
      ? props.image
      : URL.createObjectURL(props.image)
  ).then((res) =>
    res.blob().then((blob) => {
      formData.append("Kep", blob);
      fetch(`${API_BASE_URL}/Auth/profilkep`, {
        method: "POST",
        headers: {
          Authorization: `Bearer ${token}`,
        },
        body: formData,
      })
        .then(async (response) => {
          if (!response.ok) {
            throw new Error("Profilkép módosítása sikertelen");
          }
          return response.json();
        })
        .then((data) => {
          console.log(data);
          if (data.profileImageUrl != localStorage.getItem("profileImageUrl")) {
            localStorage.setItem("profileImageUrl", data.profileImageUrl);
            console.log("Profile picture updated to:", data.profileImageUrl);
            props.queryClient.invalidateQueries({ queryKey: ["me"] });
          }
        })
        .catch((error) => {
          console.error("Profilkép módosítása sikertelen:", error);
        });
    })
  );
};

const saveButtonHandler = (props: {
  username: string;
  image: File | string;
  reloadFunc: () => void;
  queryClient: QueryClient;
}) => {
  editUsername({
    username:
      (document.getElementById("usernameEdit") as HTMLElement)?.innerText ||
      props.username,
    reloadFunc: props.reloadFunc,
    queryClient: props.queryClient,
  });
  editProfilePicture({
    image:
      (document.getElementById("imagePreview") as HTMLImageElement)?.src ||
      props.image,
    reloadFunc: props.reloadFunc,
    queryClient: props.queryClient,
  });
  toast.success("Profil sikeresen módosítva!");
  document.getElementById("alertOverlay")!.style.display = "none";
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
      <ImageUpload image={props.imageUrl} setImage={() => {}} />
      <h1 contentEditable id="usernameEdit">
        {props.username}
      </h1>
      <h3>(Kattints a változtatáshoz)</h3>
      <button
        onClick={() =>
          saveButtonHandler({
            username: props.username,
            image: props.imageUrl,
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
