import { useQuery } from "@tanstack/react-query";
import styles from "../css/Recipes.module.css";
import RecipeCard from "./RecipeCard";
import { createSavedRecipeQueryOption } from "../queryOptions/createRecipeQueryOption";

const SavedRecipes = () => {
  const { data, isPending, isError } = useQuery(createSavedRecipeQueryOption());

  return (
    <div className={styles.recipes}>
      <div className={styles.recipeCardContainer}>
        {isPending && (
          <h1>
            <i className="fa-solid fa-spinner fa-spin fa-2xl"></i>
          </h1>
        )}
        {isError && <h1>Hiba történt a receptek betöltésekor.</h1>}

        {data && data.length > 0 ? (
                    data?.map((recipe) => (
                        <RecipeCard {...recipe} key={recipe.id} />
                    ))
                ) : (
                    !isPending && !isError && <p>Még nincsenek mentett receptjeid.</p>
                )}
                
            </div>
    </div>
  );
};

export default SavedRecipes;
