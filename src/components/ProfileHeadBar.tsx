import { faGear } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { useQuery } from "@tanstack/react-query";
import SettingsTab from "./SettingsTab";
import { useContext } from "react";
import { AuthUserContext } from "../context/AuthenticatedUserContextProvider";

const API_BASE_URL = "https://cbnncff2-7114.euw.devtunnels.ms";

const ProfileHeadBar = () => {
  const authUser = useContext(AuthUserContext)
  const { data: user } = useQuery({
    queryKey: ["me"],
    queryFn: async () => {
      const res = await fetch(API_BASE_URL + "/api/Auth/me", {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("access")}`,
        },
      });
      if (!res.ok) throw new Error("Failed to fetch user");
      return res.json();
    },
  });
  const handleOverlayClick = (e: React.MouseEvent<HTMLDivElement>) => {
    if (e.target === e.currentTarget) {
      const overlay = document.getElementById("alertOverlay");
      if (overlay) {
        overlay.style.display = "none";
      }
    }
  };
  return (
    <div className="headBar profileHeadBar">
      <h1 className="profileName">{localStorage.getItem("username")} receptjei</h1>
      {/* <img src={user?.profileImageUrl} alt={user?.username} className="profileImage"/> */}
      <button
        className="settingsButton"
        title="Settings"
        onClick={() => {
          const overlay = document.getElementById("alertOverlay");
          if (overlay) {
            overlay.style.display = "flex";
          }
        }}
      >
        <FontAwesomeIcon icon={faGear} />
      </button>
      <div
        id="alertOverlay"
        onClick={handleOverlayClick}
        style={{ display: "none" }}
      >
        <SettingsTab
          imageUrl={authUser?.authUser.profileImageUrl || user?.profileImageUrl || "profile.webp"}
          username={authUser?.authUser.username || user?.username || "User"}
          reloadFunc={() => window.location.reload()}
        />
      </div>
    </div>
  );
};

export default ProfileHeadBar;
