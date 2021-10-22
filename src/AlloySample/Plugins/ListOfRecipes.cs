﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlloySample.Plugins
{
    public class ListOfRecipes
    {
        public static string[] Recipes =
        {
            "Lahori Charga", "Malaysian Chicken Curry", "Stuffed Tandoori Mushroom (Bharwan Mushroom )",
            "Paneer Makhanwala", "Khariya Mundi", "Bingsu", "Chukandar Ka Halwa With Vanilla Ice Cream",
            "Chocolate Frozen Phirni", "Shahi Tirangi Kofta", "Lamb And Almond Korma", "Almond And Rose Kheer",
            "Double Chocolate Ice Cream", "Philly Cheesesteak", "Chur Chur Naan", "Nilgiri Turkey Korma",
            "Nimbu Aur Haldi Ka Duwari Murgh", "Kulle Ki Chaat", "Classic Tiramisu", "The English Garden Goblet",
            "Champaran Mutton Curry", "Laal Maas", "Hara Dana Methi Bail Gatta Curry", "Murgh Kali Mirch Ka Tikka",
            "Kesar Pista Phirni", "Murgh Malai Tikka With Mint Chutney", "Besan Ke Laddoo", "Khoya Khurchan Paratha",
            "Rose Sherbat", "Tandoori Blacked Pomfret", "Nazuk Gosht Ki Seekh", "Zaitooni Subz Biryani",
            "Rasmalai Tiramisu", "Andhra Pan Fried Pomfret", "Andhra Crab Meat Masala", "Malabari Fish Curry",
            "Ghee Roast Chicken Dosa Quesadilla", "Bamboo Biryani", "Beyond The Ozone", "Kimami Sewaiyan",
            "Awadhi Gosht Korma", "Dragon Fire Wings", "Chicken Minced Salad", "Steam Bunny Chicken Bao",
            "Lamb Barley Pot", "Ajwaini Paneer Kofta Curry", "Carrot Cake", "Kanji Vada", "Seviyan Kheer",
            "Thandai Rasmalai", "Badam Aur Gulkand Ki Kulfi", "Almond & White Chocolate Gujiya",
            "Stuffed Eggplant With Schezwan Sauce", "Ruwangan Hachi Chaman", "Hokh Hund Mutton Kofte",
            "Al Hachi Chicken", "Almond And Chicken Momos (without Shell)", "Rose Berry Valentine Forest",
            "Berry Parfait Hazelnut White Chocolate Sable", "Hariyali Biryani Risotto", "Dum Biryani Risotto",
            "Tricolor Thai Fruit Jelly", "Handi Paneer", "Stuffed Ravioli", "Marchwangan Korma", "Chemmeen Moru Curry",
            "Char Minar Biryani", "Apricot And Cardamom Dome", "Tiranga Lasagna", "Full Time LIIT", "Korean Bibimbap",
            "Strawberry Snow", "Rainbow Sangria", "Snow Shower", "Devils Love Bite", "Boozy Snake", "Ca Cang Kho",
            "Yera Sukha", "Steak Argentino", "Christmas Tree Pizza", "Portugese Fish Stew",
            "Cajun Spiced Turkey Wrapped With Bacon", "Clotted Cottage Cheese Croquettes", "Belgian Pork Chop",
            "Zucchini Halwa", "Dundee Cake", "Gajar Halwa Tart", "Winter Is Here", "Raspberry And Balsamic Dome",
            "Roast Turkey With Cranberry Sauce", "Gluten-Free Christmas Cake", "Mutton Ghee Roast",
            "Japanese Curry Arancini With Barley Salsa", "Chicken Parmigiana With Tomato Sauce",
            "Lamb And Chargrilled Bell Pepper Soup", "Cream Of Almond Soup", "Broccoli And Almond Soup",
            "Peshawari Chapli Kebab", "Mango Daiquiri", "Chocolate Appo", "Red Rice Vermicelli Kheer",
            "Lemon Honey Glazed Sous Vide Corn On The Cob", "Quinoa, Couscous And Beetroot Tikki",
            "Sous-Vide Salmon Tikka", "Roasted Spring Chicken With Root Veggies", "Gucchi Mussallam",
            "Subz Badam Ka Shorba", "Billionaire Cocktail", "Rossy Mocktail", "Peri Peri Chicken Satay",
            "Chicken Popcorn", "Tequila Sunrise", "Mahi Nazakat", "Tulsi Ajwain Ka Mahi Tikka", "Dahi Anjeer Ke Kebab",
            "Texture Of Rose Dessert", "Madata Khaja", "Gluten Free Polenta Halwa", "Multi-layered Ghevar", "Escabeche",
            "Filo Pizza", "Nazaqati Boti Kebab", "Seb Aur Badaam Ka Shorba", "Almond And Raw Banana Galawat",
            "Grilled Almond Barfi (Sugar Free)", "Chocolate Kaju Katli", "Kaju And Pista Roll", "Steamed Sandesh",
            "Ariselu", "Anjeer Kaju Roll", "Chocolate Samosa", "Mix Fruit Laccha Rabri Tortilla Crunch", "Kalmi Kebab",
            "Basanti Pulao", "Labra (Mixed Veg)", "Valencia Fizz", "Paneer Tamatar Ki Subzi",
            "Makhane Aur Kaju Ki Kheer", "Apple Rabdi", "City Of Tree", "Above The Cloud", "Malabar Fish Curry",
            "Tom Yum Prawns", "Noodle Keema Donut", "Murg Bemisal", "Satrangi Biryani", "Green Lentil Dessert Fudge",
            "Babaganoush Margarita", "Gehun Ki Kheer", "Fish Duglere", "Badam Ka Kahwa",
            "Surmai Curry With Lobster Butter Rice", "Keema Kaleji", "Mongolian Lamb Balls", "Oyster Lamb",
            "Poh Pia Je", "Vegetable Pakoda", "Crispy Calamari Rings", "Seared Salmon In Tabasco Butter",
            "Red Wine Braised Mushroom Flatbread", "Fettuccine Pomodoro", "Shakshuka",
            "Risotto Lobster With Parmesan Egg Pancake, Confit Tomatoes And Coral Tuille", "Prawn And Barley Risotto",
            "Half Roast Chicken", "Fish Skewers With Coriander And Red Wine Vinegar Dressing", "Kashmiri Halwa",
            "Baked Almond Kofta", "Badam Ki Phirni", "Almond And Amaranth Ladoo", "Murgh Biryani",
            "Pistachio Praline Parfait", "Ada Pradhaman", "Haleem Khowsuey", "Riceless Chicken Biryani",
            "Chicken Farcha", "Kashmiri Palak Aur Malai Paneer Tikka", "Lasooni Palak With Tofu And Makhani Gravy",
            "Monsoon Hunt", "Dark Night", "AK-47", "Chocolate Marquise",
            "Young Jackfruit And Water Chestnut Thai Red Curry", "Spicy Paneer With Beetroot & Cranberry Chutney",
            "Lentils, Pumpkins And Cranberry Soup", "Shrimp & Cilantro Ceviche", "Lauki Gosht", "Arbi Gosht",
            "Sangria Jelly", "Sangria Soaked Cake", "Gulkand Seviyan Kheer", "Litchi Daiquiri", "Karri Murgh Tikka",
            "Kacche Aam Ka Mahi", "Currytini", "Devil Martini", "Paneer Butter Masala", "Fresh Mango Mary",
            "Kacchi Kairi Martini", "Kanchkolar Kofta", "Dudh Puli", "Salmon With Mango & Sherry Reduction",
            "Almond Dacquoise", "Bhatti Ka Murgh", "Punjabi Sarson Da Saag", "Methi Palak",
            "Buldak (Hot And Spicy Chicken)", "Saewoo Bokumbop (Shrimp Fried Rice)", "Plum Pudding With Sabayon Sauce",
            "Vanilla Honey Pannacotta", "Chamomile Vodka Cooler", "Zesty Smoked Salmon With Avocado Tomato Salad",
            "Banana Phirni Tartlets With Fresh Strawberries", "Arbi Kaju Ki Tikki", "Sabudane Ki Kheer", "Walnut Halwa",
            "Pasta With Tangy Tomato Sauce", "Vratwale Aloo", "Makhana Kheer", "No Bake Cheesecake", "Karanji",
            "Berry Kanji", "Gaith Dal Fritters With Himalayan Chutney", "Bedmi Puri With Raseele Aloo", "Paan Thandai",
            "Kanjioska Drink", "Mango Kulfi Snow Ball", "Holi Special Malai Kofta", "Railway Mutton Curry",
            "Masala Papad", "Veg Manchurian", "Coconut And Beetroot Soup", "Exotic Lovers Roll", "Habibi",
            "Valentine Strawberry Mousse", "Strawberry Tiramisu", "Strawberry & Pistachio Breton Tart", "Love Bite",
            "Honey Sage Gin Fizz", "Strawberry Mess With Fruit Coulis", "Duo Of Chocolate And Strawberry",
            "Chicken Sukka", "Tiramisu", "Fiesole Style Potato Dumplings", "Rabri Malpua", "Stuffed Malai Kofta",
            "Kasundi Salmon Tikka With Pumpkin Pure", "Coastal Citrus And Basil", "Achari Paneer Tikka",
            "Keerai Masial With Carrot And Coconut Rice", "Smoked Salmon And Four Cheese Focaccia", "Tricolor Dimsums",
            "Tricolor Seekh Kebab", "Tiranga Pulao", "Tricolor Cottage Cheese Skewers", "Tiranga Halwa",
            "Tricolour Pizza", "Chawal Ki Kheer", "Kesar Pista Phirni", "Roasted Tomato And Herb Soup",
            "Patishapta With Strawberry Coulis", "Steamed Chicken Roulade", "Goan Prawn Curry & Baked Rice Canapes",
            "Easy Fruit Custard", "Veg Fried Rice", "Lauki Kofta", "Mustard-Parmesan Whole Roasted Cauliflower",
            "Slow Cooked Spiced Sangria", "Matar Paneer Masala", "Methi Chicken Masala", "Flax Seed Raita",
            "Spicy Gatte Ki Sabzi", "Kachri Qeema", "Restaurant Style Fried Chicken", "Homemade Gulab Jamun",
            "Shahi Mushroom Masala", "Stollen Bread", "Chicken Shami Kebab", "Mother Christmas Cake",
            "Christmas Pudding", "Allahabadi Tehri", "Anjeer Ki Barfi", "Matcha Tea Macarons", "Whisky Chai",
            "Panettone", "Linzer Cookies", "Christmas Brownies", "Punjabi Style Nutri Kulcha", "Chicken Potli",
            "Roasted Chicken Masala", "Spicy Chicken Masala", "Shahi Paneer", "Spicy Chicken Curry",
            "Punjabi Dal Tadka", "Palak Paneer", "Paneer Tikka Roll", "Mushroom Palak Kofta", "Mutton Shami Kebab",
            "Spicy Paneer Tikka", "Mango Ice Cream", "Fresh Water Chestnut Panacotta With Pecan Nut And Honeycomb",
            "Crispy Herb Chicken", "Instant Rava Dosa", "Gajar Ka Halwa With Coffee Gulab Jamun", "Rasgulle Ki Sabzi",
            "Spooky Lamb Pie With Glazed Carrot.", "Halloween Chorizo And Goat Cheese Risotto", "Mummy Dogs",
            "Spider Cake", "Mummy Wraps (Tortellini Stuffed With Minced Meat)", "Chopped Witch Fingers",
            "Turkish Tulumba", "Veg Summer Rolls", "Khoya Paneer", "Dahi Aloo", "Tamater Ki Launji", "Summer Breeze",
            "Fall Cocktail", "Bourbon Fig", "Mutton Beliram", "Nuggekai Pulimunchi", "Khas Khas Ka Halwa",
            "Kachori Upside Down", "Aanarsa", "Assorted Rice Kheer Sushi", "Saffron Cardamom Panacotta",
            "Jalebi With Fennel Yogurt Pudding", "Zaffrani Rabri Crme Anglasise With Lauki Ka Lacche", "Jaggery Jamun",
            "Faldhari Badam Ki Barfi (Sugar Free)", "Egg Masala Curry", "Dum Paneer", "Banarasi Dum Aloo",
            "Dry Fruit Modak", "Dahi Chicken", "Coconut Ladoo", "Chilli Paneer Dry", "Bengali Style Chana Dal",
            "Bagara Baingan Masala", "Aata Cake", "Amritsari Chicken Masala", "Achaari Aloo", "Dahi Aur Bhindi",
            "Soya Chaap Curry", "Bihari Fish Curry", "Easy Dahi Bhalla", "Chilli Chicken", "Hara Bhara Kebab",
            "Dhaba-Style Chicken", "Keema Matar Masala", "Spaghetti With Clams & Crispy Bread Crumbs", "Margarita",
            "Strawberry Margarita", "Bailey's Delight", "Baked Vegetables Casserole", "Chicken Nimbu Dhaniya Shorba",
            "Dahi Lasooni Chicken", "Garlic Soya Chicken", "Beer Batter Fish Fingers", "Greek Style Pizza",
            "Quinoa Risotto With Mushroom", "Lauki Gulkand Barfi", "Garlic Prawns", "Cauliflower And Chicken Biryani",
            "Chicken Quinoa Biryani", "Wok Tossed Asparagus In Mild Garlic Sauce", "Mixed Sprouts Corn Chaat",
            "Capsicum Masala", "White Chocolate Parfait With Berry Compote", "Ambur Mutton Biryani",
            "Pineapple Upside Down Cake", "Cardamom Biscotti", "Fudgy Chocolate Brownies",
            "Caramel Spiced Mango Mousse", "Mango Lassi Ice Cream", "Lemon Chicken And Rocket Pasta",
            "Baked Nut Crusted Halibut", "Chicken And Mushroom Lasagna", "Chettinad Kozhi (Chicken) Curry",
            "Love, Live And Laugh", "Hummus With Avocado Rolls", "Ratatouille Nicoise",
            "Crema Di Cannellini Con Mescolare Funghi (Mushroom Soup)", "Coq Au Vin (Chicken Braised In Wine)",
            "Lagan Ka Gosht", "Rampuri Korma", "Aloo Qorma", "Zucchini Methi Pulao", "Chicken Roulade",
            "Sticky Rum Chicken Wings", "Haleem", "Jowar Tacos With Spicy Chicken Filling", "Mushroom Clear Soup",
            "Tomato Papeta Par Eeda", "Chicken 65", "Dhaba Raan", "Dhaba Murg Roast", "Genie In A Bottle",
            "Tawa Surmai", "Tawa Mutton", "Tawa Sabz Pulao", "Eggless Banana Cake", "Eggless Truffle Cake",
            "Eggless Truffle Cake", "Atta Rann", "Tabriz Kofta", "Chemeen Thoren", "Malai Kofta",
            "Corn & Cauliflower Soup", "Eggless Almond And Cashew Cake", "Deviled Eggs", "Spicy Creamy Kadai Chicken",
            "Prawn Tikka Masala", "Hyderabadi Dum Ka Murgh", "Low Fat Pepper Chicken Dry",
            "Missi Roti Crisp With Apricot Chutney And Cream Cheese", "Khade Masale Ki Chaap",
            "Sali Boti (Parsi Meat Dish)", "Sheer Khurma", "Bhuni Raan", "Kerala Roast Chicken",
            "Konju Varutharaccha Curry (Kerala Prawn Curry)", "Paneer Chaman", "Le Turinois (Chocolate Chestnut Cake)",
            "Fish In Green Masala", "Chicken Layonnaise", "Paya Curry", "Hyderabadi Biryani", "Potato Chops With Keema",
            "Smokey Meat Curry", "Microwave Machchli Biryani", "Chicken Paella", "Makhni Paneer Biryani",
            "Thai Fish Curry", "Shahi Tukda", "Codfish Salad", "Coconut Gujiya", "Kaithine Pachile", "Shrimp Diane",
            "Chicken Cutlets With Panada Sauce", "Bacon And Singhada Rolls", "Seekh Kebabs", "Chimney Soup",
            "Kashmiri Rogan Josh", "Jhinga Pulao", "Prawn Biryani", "Pan Fried Fish With Almonds",
            "Konkani Grilled Fish", "Spiced Moorish Lamb Kebab", "Eggless Mango Mousse", "Fish Mur-Moro",
            "Awadhi Mutton Biryani", "Double Ka Meetha", "Aubergine And Green Chili Salan", "Chicken Cafreal",
            "Paneer In White Gravy", "Paneer Afghani", "Mysore Pak", "Eggless Chocolate Cake", "Chicken Manchurian",
            "Tiramisu", "Fish Orly", "Doi Machch", "Pork Chops", "Stir Fried Prawns", "Crepe Suzette",
            "Garlic Lamb Chops", "Roasted Aubergine Dip", "Southern Style Okra", "Kesari Jalebi", "Apple Kheer",
            "Paneer Tikki", "Erissery", "Pulissery", "Avial Curry", "Chocolate Chip Cheesecake", "Machchli Masala",
            "Chilli Fish", "Omelette Curry", "Pork Curry", "Lamb Korma", "Cardamom Lamb Curry", "Methi Chicken",
            "Baked Chicken Seekh", "Punjabi Lemon Chicken", "Light Chicken Fiesta", "Moist Chocolate Cake",
            "Pesto Chicken", "Green Chilly Raita", "Talwa Paratha", "Savory Corn Tarts", "Salted Caramel Sauce",
            "Punjabi Meat Masala", "Shaami Kebab", "Katra Style Mutton Curry", "Zucchini Stuffed With Soya",
            "Honey Glazed Mock Duck", "Andhra Style Chicken Curry", "Parsi Mutton Cutlets", "Sali Marghi",
            "Chicken Balls In Yakitori Sauce", "Pommes Gratin", "Dahi Bhalla", "Chocolate Lava Cake", "Mutton Fry",
            "Mutton Biryani", "Hot Paneer Sandesh Pudding", "Beetroot And Coconut Soup",
            "Rice Noodles With Stir Fried Chicken", "Garlic And Egg Fried Rice",
            "Pasta With Roasted Mediterranean Veggies", "Kerala Chicken Roast", "Spicy Singapore Noodles",
            "Plum And Green Apple Yogurt", "Peanut Sauce", "Cheese Chicken Kebabs", "Gulabi Phirni",
            "Fish Curry With Lotus Stem", "Kashmiri Chicken Pulao", "Chicken Dum Biryani", "Bharwan Chicken Pasanda",
            "Rasmalai", "Spicy Prawns With Sweet Dipping", "Chicken Mascarpone", "Vietnamese Cold Spring Rolls",
            "Tiramisu - The 'pick-me-up' Cake", "Chocolate Pizza", "Modak", "Chicken Xacuti", "Eggless Coffee Cupcakes",
            "Penne Ala Vodka", "Seafood Spaghetti", "Kuttu Ki Puri", "Cinnamon Rolls", "Dark Chocolate Tart",
            "California Sushi Rolls", "Low Fat Butter Chicken", "Dum Paneer Kali Mirch",
            "Imran Khan's Chilli Con Carne", "Mushroom Brown Rice", "Achaar Ka Paratha",
            "Cous Cous Studded Cottage Cheese", "Cheese Fondue", "Duck A L'orange", "Chicken Tangdi", "Raan Musallam",
            "Ham Rolls", "Paneer Kofta", "Dried Fruit Pulao", "Cheesy Spinach Roulade", "Rasgulla", "Lemon Pudding",
            "Chicken In White Wine", "Shahi Egg Curry", "Chicken Curry In Coconut Milk", "Chicken Pizza",
            "Minced Meat Burritos", "Lamb Rogan Josh", "Fish Kebabs With Sauce And Brown Rice", "Roast Pork Belly",
            "Barley Risotto With Marinated Chicken", "Chicken Satay", "Spicy Malvani Chicken Curry", "Mutta Aviyal",
            "Potato Chicken Stew", "Rice In Lamb Stock", "Rose Petal Rice", "Moong Dal Payasam",
            "Potato With Lotus Stem", "Kheer Khas Khas", "Amritsari Murgh Makhani", "Methi Machchi", "Garlic Paratha",
            "Chicken Rogan Josh", "Chicken Korma With Coconut Milk", "Chicken In Tomato Gravy", "Lauki Mussallam",
            "Bengali Lamb Curry", "Fish Curry", "Dahi Kebab", "Chicken Do Pyaaza", "Goat Brain Cutlets",
            "Raw Banana Kebab", "Prawn Coconut Curry", "Prawn Potato Soup", "Mushroom Kofta In Tomato Gravy",
            "Murgh Malaiwala", "Hot And Sour Soup", "Banh Cuon (Vietnamese Dumplings)", "Yellow Chicken Curry",
            "Lamb With Beans Thoran, Potato Curry And Rice", "Honey Chicken Wings", "Mutton Kathi Roll",
            "Stuffed Zucchini Boats", "Chicken Barbequed Skewers", "Cabbage Kofta", "Paneer Tikka",
            "Chicken With Chorizo", "Mushroom Chettinad", "Shahi Mushroom", "Amritsari Paneer Bhurji", "Zafrani Pulao",
            "Shahi Tukda", "Almond Malai Kulfi", "Mango And Mint Kheer", "Tandoori Chicken",
            "Masaledar Chicken Lollipop", "Stuffed Masala Mushrooms", "Fish Cutlets", "Hara Masala Kebabs",
            "Prawn Rava Fry", "Karare Murgh Ke Pasande", "Stuffed Papad", "Mutton Kathi Roll", "Egg Chaat",
            "Aloo Tikki", "Mutton Cutlets", "Chicken 65", "Chole Bhature", "Pahadi Murgh", "Goan Crab Curry",
            "Spicy Tangy Kadhai Chicken", "Egg Masala", "Khade Masala Ka Ghosht", "Handi Biryani", "Margherita Pizza",
            "Galouti Kebab", "Badami Murgh", "Hare Masala Da Champ", "Neni Rogan Josh", "Dhaniwal Korma (Lamb Korma)",
            "Dark Leafy Greens With Caramelized Onions, Raisins, And Maple Walnuts", "Melon Saketini",
            "Mediterranean Martini", "Chocolate Martini", "Bellini", "Kurumba Sunset", "Kurumba Passion",
            "Hemmingway Air", "Grilled Lemon Margarita", "Chrysanthemum Infusion Shot", "Chocolate Rum Cake",
            "Quick Chicken Curry", "Chicken Seekh Kebabs", "Pav Bhaji", "Chocolate Jaffa Mousse", "Chilli Soya Nuggets",
            "Falafel With Pita Bread", "Chicken Chopsuey", "Rara Chicken", "Angoor Rabdi", "Eggless Pineapple Pastry",
            "Thandai Ice Cream", "Pumpkin And Apple Halwa", "Khao Soi", "Plum Cake", "Sabudana Kheer",
            "Melting Chocolate Surprise", "Chocolate Cream Pudding", "Lagan Nu Custard", "Saffron Scented Apples",
            "Eggless Marble Cake", "Sonth Aur Methi Ka Ladoo (Laddu)", "Imarti", "Caramel Custard In A Pressure Cooker",
            "Paneer Makhani", "Mutton Goli Biryani", "Kaju Curry", "Goan Egg Curry", "Beer Battered Mushrooms",
            "Bhein Ke Kebab", "Tofu Phali", "Paneer Goli Biryani", "Paani Poori", "Malai Kofta Dum", "Padampuri Murg",
            "Gushtaba", "Rista", "Chicken Fritters", "Paneer Tikka Kathi Roll", "Paneer Sizzler", "Murgh Lahori Kadhai",
            "Balochi Gosht", "Keema Ke Kofte", "Gujiya", "Gulab Jamun Cheesecake", "Besan Ke Ladoo (Laddu)", "Petha",
            "Boondi Ke Ladoo (Laddu)", "Meen Murringakka Curry", "Marble Cake", "Marchwagan Korma",
            "Curried Lamb Strips", "Khumb Palak Kofta", "Chicken Kolhapur", "Kofta Curry",
            "Karoo Lamb Cutlets In A Rosemary And Port Jus", "Gajar Ka Halwa", "Falafel Wrap", "Egg Curry",
            "Dum Aloo Lakhnavi", "Dhaniwal Korma", "Coriander And Spinach Chops", "Paal Payasam",
            "Hyderabadi Mirchi Ka Salan", "Dum Biryani", "Chicken Chettinad", "Chicken Villeroy",
            "Boulettes De Moutons Aux Epinards", "Blueberry Cheesecake", "Badam Korma", "Badaam Pasanda", "Anari Champ",
            "Alubukhar Korma", "Almond And Lamb Biryani", "Achaar Ke Aloo", "Turkish Kofte", "Tabakh Maaz",
            "Sweet Chicken Curry", "Spicy Chicken", "Reshmi Kebabs", "Red Seer Fish Curry", "Seafood Rasam",
            "Roasted Chicken With Mushroom And Wine Sauce", "Rogan Josh", "Malai Prawn Curry",
            "Prawns In Spiced Butter", "Prawn Rissois", "Peppercorn Meat", "Parsi Fish Curry", "Mustard Lamb",
            "Multani Paneer Tikka", "Moroccan Lamb", "Modak", "Leftover Idli Snack", "Peshawari Chicken Kebab",
            "Curried Scallops", "Malwani Chicken Sukka", "Jehangiri Kebab With Duck Breast", "Trio Of Kulfis",
            "Jheenga Aloo Kofta With Tamarind Rice", "Fruit Chaat Bites", "Potatoes Wrapped In Crispy Rice Pancake",
            "Lahori Tawa Tali Machchli", "Meen Alleppey Curry With Brown Rice", "Babycorn Bezule With Peanut Chutney",
            "Karare Chaawal Aur Macchi Ki Tikki", "Paneer Kadhai Masala And Ajwain Rotis",
            "Poached Lahori Fish With Sun Blushed Red Chillies And Imli Pesto", "Roomali Paneer And Chutney Butter",
            "Tandoori Prawns Chaat, Chakotra, Laal Mooli And Ganth Gobhi Salad", "Steamed Fish", "Tomato Egg Curry",
            "Pepper Mushrooms With Tomato Chutney", "Tawa Tadka Keema", "Samandari Khazana", "Gur Aur Atte Ka Halwa",
            "Sarson Ka Saag With Bajre Ki Roti", "Sevian With A Peach Murabba", "Palak Murgh Roulade With Makhni Gajar",
            "Malai Ki Kheer", "Murg Palak Ke Korma Kebab", "Miso Coconut Basa", "Mutton Dhansak", "Kaghzi Kebab",
            "Achaari Murgh With Desi Ghee Khichdi", "Steamed Crab With Tomato And Dal Rasam",
            "Gajar Halwa Sunehri Style", "Keema Kaleji Ki Tikki And Khubani Ki Chutney", "Four Cheese Pasta",
            "Barley Risotto", "Chicken Butter Masala", "Chicken Pakoda", "Spicy Chicken", "Nimbu Hari Mirch Ka Murgh",
            "Guilt Free Karwari Prawns", "Potatoes Bhujia With Besan Ki Roti", "Chicken Chaska From Gawal Mandi",
            "Chicken Kolhapuri", "Bhatata Ani Jhinga Rassa With Phodni Cha Maka", "Chicken Khada Masala",
            "Murgh Makhanwala", "Chatpattey Coconut Kebab", "Tellicherry Pepper Prawns Fry With Saboodana Papad",
            "Meen Moilee With Steamed Rice", "Guilt Free Chicken Tacos", "Tomato Salsa", "Anda Kaleji",
            "Dimer Dhokkar Dalna And Luchi", "Shikarpuri Chatkhana Karhai", "Meen Vazhakkai Chops", "Dhaniya Murgh",
            "Dum Murgh Keema Baingan", "Chicken Shaami Kebab", "Alleppey Chemeen Curry", "Stuffed Baby Eggplant",
            "Khao Suey", "Multigrain Pizza", "Hariyali Machli With Onion Pulao", "Masala Machi Bites With Apple Confit",
            "Meen Curry With Ghee Bhaat", "Maaun Ke Machchey", "Kabuli Pulao", "Dhoowan Gosht",
            "Tandoori Tikka Seel Kand", "Chana Aur Khatte Pyaaz Ka Murgh", "Laziz Lamb Handi", "Pyazi Kebabs",
            "Peshawari Chappali Kebab", "Nihari Gosht With Varqi Paratha", "Aatukkari Kuzhambu With Steamed Rice",
            "Seekh Kebab With Seb Pyaaz Ki Chaat", "Cream Chicken", "Lemon Chicken", "Warli Style Mutton Curry",
            "Honeyed Prawns", "Thai Steamed Fish", "Golden Rasmalai", "Kholdaar Machli Biryani",
            "Chinioti Mutton Machhli", "Bhindi Do Rukha", "Laksa Chicken Tikka", "Yogurt Kebab",
            "Guilt Free Galouti Kebab", "Pasta In Spinach Sauce", "Cauliflower Bake", "Ulte Tawe Ka Paratha",
            "Rocket Salad", "Roasted Duck", "Spinach & Cauliflower Soup", "Patra-Ni-Machchi",
            "Himachali Grilled Chicken", "Chikhali Style Dal Gosht", "Aggari Style Lobster Curry",
            "Shiitake Fried Rice With Water Chestnuts", "Panko Crusted Cottage Cheese", "Watermelon Curry",
            "Cottage Cheese Souvlaki", "Eggplant Parmigiana", "Green Chilly And Raw Mango Risotto",
            "Fenugreek And Lal Maat Crostini", "Christmas Dry Fruit Cake", "Chicken Masala", "Chicken Malai Makhni",
            "Calicut Chicken Biryani", "Murgh Badam Korma", "Coq Au Vin", "Nimpudu Kodi", "Andhra Pepper Chicken",
            "Achaari Chicken", "Murgh Aatish Burra", "Safed Murgh", "Murgh Methi Malai", "Dhaba Chicken",
            "Chicken Saagwala", "Low Fat Murgh Kurkuri", "Ginger Chicken", "Pate Maison", "Pan Fried Chicken",
            "Chicken In Net Parcels", "Butter Chicken", "Roast Chicken With Apricot & Mint Stuffing",
            "Gardener's Chicken", "Chicken Yogurt Curry", "Pahadi Chicken", "Coconut And Litchi Creme Caramel",
            "Chicken Piccata With Bread Salad", "Salt Crusted Baked Snapper", "Sausage Pepper Burger",
            "Spaghetti Meatballs", "Apple Pie With Raisin Relish", "Nihari Gosht", "Tiramisu", "Rice Kheer",
            "Mutton Korma", "Beetroot Soup", "Low Fat Makhana Kheer", "Tamatar Kadhi", "Chatpata Paneer",
            "Eggless Vanilla Cake In A Pressure Cooker", "Egg Biryani", "Punjabi Samosa", "Baby Lobster In White Sauce",
            "Fish Biryani", "Mutton Kofta", "French Onion Soup With Cheese Souffle", "Orange Cake",
            "Black Forest Gateau", "Eggless Atta Cake", "Dark Chocolate Mousse With Amarula Creme",
            "Vegetable Chowmein", "Millefeuille Of Aubergine", "Navrattan Korma", "Boti Kebab", "Murgh Mussallam",
            "Vegetable Fried Rice", "Keema Dum", "Roomali Roti", "Ghutti Hui Gobhi", "Bhuni Kaleji", "Murgh Do Pyaaza",
            "Korma-E-Vakil", "Murgh-E-Kalmi", "Vegetable Chopsuey", "Murgh Shahi Korma", "Spicy Pav Bhaji",
            "Tuna Stuffed Eggs", "Ajo Blanco", "Chicken Sweet Corn Soup", "Cream Of Broccoli Soup", "Fruit Cupcakes",
            "Scone Pizza", "Singapore Chilli Crab", "Tandoori Chicken Sticks", "Banana Cake",
            "Eggless Vanilla Cake In A Microwave", "Pindi Channe", "Khatta Meetha Baingan", "Kadai Paneer",
            "Dal Makhani", "Khatti Meethi Dal", "Rajma (No Onion, No Garlic)", "Gatte Rasedaar",
            "Matar Paneer Lababdaar", "Khatte Channe", "Ghiya Ke Kofte", "Khus-Wala Matar", "Undhia", "Shahi Paneer",
            "Stuffed Aubergines With Lamb", "Chicken Pie", "Singara Kalia", "Aloo Ka Paratha", "Lal Paneer",
            "Microwave Bhuna Keema", "Microwave Murg Manpasand", "Tangri Kebabs", "Microwave Chicken Steak",
            "Microwave Baingan Ka Bharta", "Microwave Machchli Lal Masala", "Fish With Tomatoes", "Paneer Jalfrezi",
            "Noodles With Mixed Meat", "Sausage Wontons", "Gobhi Mussallam", "Microwave Chicken Biryani", "Ker Sangri",
            "Lamb In Hot And Sour Sauce", "Palak Ka Saag", "Asparagus With Creamy Mushrooms", "Paneer Achaari",
            "Mutton Dry", "Chicken Salad", "Vegetable Jalfrezi", "Palak Paneer", "Mutton Stew", "Minestrone Soup",
            "Barbecue Fish Tikka", "Ratatouille Moulds With Saffron Sauce", "Kakori Kebab",
            "Mushroom And Herb Filled Tomatoes", "Roast Chicken", "Khamiri Roti", "Lachcha Paratha", "Baqarkhani Roti",
            "Paav", "Sheermal", "Chapati", "Babru", "Methi Ka Thepla", "Naan", "Saada Paratha", "Mughlai Paratha",
            "Varki Paratha", "Governor's Chicken", "Chicken Afghani", "Peranakan Chicken Curry", "Chicken Lollipops",
            "Chicken In Lemon Sauce", "Boneless Chilli Chicken", "Zeera Chicken", "Baked Spinach And Corn",
            "Cheese Souffle", "Vegetarian Nargisi Kofta", "Vegetable Au Gratin", "Chilli Paneer", "Meat Lasagne",
            "Cheese And Ham Roll", "Cheese And Lamb Steaks", "Pot Roasted Chicken", "Chicken Kabi Raji",
            "Turkey Breast With Orange Reduction", "Dorro Wot", "Chicken Steak", "Vada Kombda", "Kalmi Kebab",
            "Chicken Nizami", "Chicken Kathi Roll", "Chicken Shashlik Sizzler", "Bataer (Quail) Rasedaar",
            "Prawn Temperado", "फिश विद टोमैटो", "चिली चिकन मसाला", "ढाबा स्टाइल पनीर", "मटन का हरा कीमा",
            "ब्लैक मटन करी", "चिकन तेरियाकी", "सेसमे चिकन", "चिकन निजामी", "हरियाली मुर्ग मसाला", "टोमैटो चिकन",
            "मुर्ग रसेदार", "वेजिटेरिन शामी कबाब", "मुगलई एग", "होममेड चिकन निहारी", "मुर्ग मुस्सलम", "गोश्त का सालन",
            "वेज कीमा", "दही लहसुनी चिकन टिक्का", "चिकन सीक कबाब पुलाव", "एग कीमा पुलाव", "मटन सुक्का"
        };
    }
}