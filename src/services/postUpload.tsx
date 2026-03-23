import type { ContentType } from "../types/ContentType";
import type { ApiContentType } from "../types/ApiContentType";

const API_BASE_URL = "https://cbnncff2-7114.euw.devtunnels.ms/api";
let allTags: { cimkeNev: string; cimkeId: number }[] = [];
const accessToken = localStorage.getItem("access") || "";

export const getTags = async () => {
  try {
    const response = await fetch(`${API_BASE_URL}/Cimkek`);

    if (!response.ok) {
      throw new Error("Failed to fetch tags");
    }

    return await response.json();
    // console.log("Fetched tags:", allTags);
  } catch (error) {
    console.error("Error:", error);
  }
};
const postUpload = async (rawPost: ContentType) => {
  let TagIdList: { id: number }[] = [];


  const checkIfTagExists = async (tagToCheck: string) => {
    try {
      if (
        allTags.some((tag: { cimkeNev: string }) => tag.cimkeNev === tagToCheck)
      ) {
        console.log("Van ilyen címke");
        return true;
      } else {
        console.log("Nincs ilyen címke");
        return false;
      }
    } catch (error) {
      console.error("Error uploading tag:", error);
    }
  };

  const uploadTag = async (token: string, tagToUpload: string) => {
    try {
      const response = await fetch(`${API_BASE_URL}/Cimkek`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({ cimkeNev: tagToUpload }),
      });

      if (!response.ok) {
        throw new Error("Failed");
      }
      const data = await response.json();
      console.log(data);

      TagIdList.push({ id: data.cimkeId });
    } catch (error) {
      console.error("Error fetching tags:", error);
    }
  };

  // console.log(accessToken);
  allTags = await getTags();
  if (rawPost.tags && rawPost.tags.length > 0) {
    for (let index = 0; index < rawPost.tags.length; index++) {
      for (let i = 0; i < allTags.length; i++) {
        if (allTags[i].cimkeNev == rawPost.tags[index]) {
          TagIdList.push({ id: allTags[i].cimkeId });
          break;
        }

      }
      if ((await checkIfTagExists(rawPost.tags[index])) == false) {
        await uploadTag(accessToken, rawPost.tags[index]);
        console.log("feltöltve");
        } else {
        console.log("nem kell feltölteni");
      }
    }
  }


  console.log(TagIdList);

  let postToUpload: ApiContentType = {
    Nev: rawPost.title,
    CimkeIds: TagIdList.map((tag) => +tag.id),
    ElkeszitesiIdo: rawPost.length,
    Hozzavalok: rawPost.ingredients,
    Leiras: rawPost.description,
    NehezsegiSzint: rawPost.difficulty,
    Kep: rawPost.image,
  };

  const uploadPost = async (token: string, postToUpload: ApiContentType) => {
    try {
      const formData = new FormData();

      formData.append("Nev", postToUpload.Nev);
      formData.append("ElkeszitesiIdo", postToUpload.ElkeszitesiIdo.toString());
      formData.append("Hozzavalok", postToUpload.Hozzavalok || "");
      formData.append("Leiras", postToUpload.Leiras || "");
      formData.append("NehezsegiSzint", postToUpload.NehezsegiSzint || "");

      postToUpload.CimkeIds.forEach((id) => {
        formData.append("CimkeIds", id.toString());
      });

      if (postToUpload.Kep) {
        formData.append("Kep", postToUpload.Kep);
      }

      console.log("FormData contents:");
      for (let [key, value] of formData.entries()) {
        console.log(`${key}:`, value);
      }

      const response = await fetch(`${API_BASE_URL}/Recept/with-image`, {
        method: "POST",
        headers: {
          Authorization: `Bearer ${token}`,
        },
        body: formData,
      });

      if (!response.ok) {
        const errorText = await response.text();
        console.error("Response status:", response.status);
        console.error("Error details:", errorText);
        throw new Error(`Failed: ${response.status}`);
      }

      const data = await response.json();
      console.log("Response data:", data);
      console.log("Sikeres post feltöltés");
      return data;
    } catch (error) {
      console.error("Error uploading post:", error);
      throw error;
    }
  };
  await uploadPost(accessToken, postToUpload);
};

export default postUpload;
