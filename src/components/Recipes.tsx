import { useQuery } from '@tanstack/react-query'
import createRecipeQueryOption from '../queryOptions/createRecipeQueryOption'
import styles from '../css/Recipes.module.css'
import RecipeCard from './RecipeCard'


const Recipes = () => {
  const {data, isPending, isFetched, isError} = useQuery(createRecipeQueryOption())
  return (
    <div className={styles.recipes}>
        <h1>Legújabb receptek</h1>
        <div className={styles.recipeCardContainer}>
          {isPending && <h1><i className="fa-solid fa-spinner fa-spin fa-2xl"></i></h1>}
          {isFetched && data?.map((recipe) => (<RecipeCard {...recipe} key={recipe.id} />))}
          {isError && <h1>Hiba</h1>}
        </div>
    </div>
  )
}

export default Recipes