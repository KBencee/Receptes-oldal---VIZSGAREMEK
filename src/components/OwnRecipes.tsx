import { useQuery } from '@tanstack/react-query'
import createOwnRecipeQuery from '../queryOptions/createOwnRecipeQuery'
import styles from '../css/Recipes.module.css'
import RecipeCard from './RecipeCard'

const OwnRecipes = () => {
    const { data, isPending, isError } = useQuery(createOwnRecipeQuery())

    return (
        <div className={styles.recipes}>
            <h1>A saját recepteid</h1>
            <div className={styles.recipeCardContainer}>
                
                {isPending && (
                    <h1><i className="fa-solid fa-spinner fa-spin fa-2xl"></i></h1>
                )}
                {isError && <h1>Hiba történt a receptek betöltésekor.</h1>}

                {data && data.length > 0 ? (
                    data?.map((recipe) => (
                        <RecipeCard {...recipe} key={recipe.id} />
                    ))
                ) : (
                    !isPending && !isError && <p>Még nincsenek feltöltött receptjeid.</p>
                )}
                
            </div>
        </div>
    )
}

export default OwnRecipes