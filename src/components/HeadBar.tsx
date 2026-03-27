import styles from "../css/Headbar.module.css";
import type { RecipeType } from "../types/RecipeTypes";
import {
  searchRecipesByTags,
  searchRecipesByTitle,
} from "../queryOptions/createRecipeQueryOption";
import { getTags } from "../services/postUpload";
import { useEffect, useState } from "react";

let allTags: { cimkeNev: string; cimkeId: number }[] = [];
allTags = await getTags();
const HeadBar = ({
  onSearch,
}: {
  onSearch: (results: RecipeType[]) => void;
}) => {
  const [term, setTerm] = useState("");

  useEffect(() => {
    const timeout = setTimeout(() => {
      handleSearch(term);
    }, 300);
    return () => clearTimeout(timeout);
  }, [term]);

  const handleSearch = async (term: string) => {
    if (term != "") {
      const tempResults = await searchRecipesByTitle(term);
      for (let i = 0; i < allTags.length; i++) {
        if (allTags[i].cimkeNev.toLowerCase().includes(term.toLowerCase())) {
          tempResults.push(...(await searchRecipesByTags(allTags[i].cimkeId)));
        }
      }
      const deduplicatedResults = tempResults.filter(
        (recipe, index, self) =>
          index === self.findIndex((r) => r.id === recipe.id)
      );
      onSearch(deduplicatedResults);
    } else {
      onSearch([]);
    }
  };
  return (
    <div className={styles.headBar}>
      <h1>Receptek</h1>
      <input onChange={(e) => setTerm(e.target.value)} />
    </div>
  );
};

export default HeadBar;
