using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MLNR.Engine.Utilities;
using MLNR.Engine.Entities;
using MLNR.Engine.Resources;
using MLNR.Engine.GameComponents;
using MLNR.Engine.Entities.Geometry;
using MathNet.Numerics.LinearAlgebra;
using MLNR.Engine.AI;
using System.IO;
using System.Text.RegularExpressions;

namespace MLNR.Engine.Level
{
    static class Playfield
    {
        public static List<Entity> Entities { get; private set; }
        public static List<Entity> EntityBuffer { get; set; }
        public static Vector2 CameraLocation { get; private set; }
        public static Random RNG { get; set; }

        static Input input;

        static Vector2 topLeft;
        static int height;
        static int size;

        static NeuralNetwork neuralNetwork;

        static Shape outline;
        static Text text;

        static int currentNumber;
        
        static List<Data> savedData;
        static int guess;
        static int confidenceLevel;

        static bool train;
        static Timer drawPause;

        public static void Initialize()
        {
            Entities = new List<Entity>();
            EntityBuffer = new List<Entity>();
            RNG = new Random(DateTime.Now.Millisecond);
            input = new Input(PlayerIndex.One);
            drawPause = new Timer(100);

            savedData = new List<Data>();
            currentNumber = 0;
            LoadData();

            Reset();
        }

        private static void LoadData()
        {
            string[] data = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "Data/data.txt");

            string[] line = new string[0];
            double[] input = new double[21 * 21];
            double[] output = new double[10];

            foreach (string s in data)
            {
                line = Regex.Split(s, " ");
                input = new double[21 * 21];
                output = new double[10];

                int i = 1;
                for (; i < 1 + 21 * 21; i++)
                {
                    input[i - 1] = int.Parse(line[i]);
                }
                for (; i < line.Length - 1; i++)
                {
                    output[i - (1 + 21 * 21)] = int.Parse(line[i]);
                }
                savedData.Add(new Data(input, output));
            }
        }

        public static void Reset()
        {
            Entities.Clear();

            //neuralNetwork = new NeuralNetwork(21 * 21, 2, 25, 10);
            neuralNetwork = new NeuralNetwork("Data/Neural_Network");

            height = 21;
            size = (int)(Camera.ScreenBounds.Height * 0.8f / height);

            topLeft = new Vector2((Camera.ScreenBounds.Width - size * height) / 2, (Camera.ScreenBounds.Height - height * size - 4 - 8) / 2);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < height; x++)
                {
                    Entities.Add(new Shape(topLeft.X + x * size, topLeft.Y + y * size, size - 1, size - 1, new Color(Color.White, 0)));
                }
            }

            outline = new Shape(topLeft.X - 2, topLeft.Y - 2, height * size + 4, height * size + 4, 2, Color.White);
            text = new Text(topLeft.X, topLeft.Y + height * size + 6, "This looks like the number ...", Sprite.Type.Text8x8);
        }

        private static void ResetBoard()
        {
            foreach (Entity e in Entities)
            {
                e.SetColor(new Color(Color.White, 0));
            }
        }

        private static void BackToMenu()
        {
            Reset();
            Game1.GameMode = Game1.Mode.Menu;
        }

        private static void CameraHandler(GameTime gameTime)
        {
            Camera.Update(CameraLocation, 0, Camera.ScreenBounds.Width * 10000, 0, Camera.ScreenBounds.Height);
        }

        private static void UpdateInput(GameTime gameTime)
        {
            input.Update(gameTime);

            if (input.KeyState.IsKeyDown(Keys.Space) && input.KeyReleased)
            {
                ResetBoard();
                input.KeyReleased = false;
            }
            if (input.KeyState.IsKeyDown(Keys.LeftControl) && input.KeyReleased)
            {                
                train = !train;
                input.KeyReleased = false;
            }

            if (input.KeyState.IsKeyDown(Keys.Enter) && input.KeyReleased)
            {
                neuralNetwork.Save();
                input.KeyReleased = false;
            }
            if (input.KeyState.IsKeyDown(Keys.LeftShift) && input.KeyReleased)
            {
                SaveNumber(currentNumber);
                input.KeyReleased = false;
            }

            SetCurrentNumber();

            int x = (int)((input.DynamicCursorLocation.X - topLeft.X) / size);
            int y = (int)((input.DynamicCursorLocation.Y - topLeft.Y) / size);

            //Console.WriteLine("{0} {1}", x, y);

            if (input.LeftClick() && x >= 0 && x < height && y >= 0 && y < height)
            {
                Entities[y * height + x].SetColor(new Color(Color.White, 255));
                drawPause.Reset();
            }
            if (input.RightClick() && x >= 0 && x < height && y >= 0 && y < height)
            {
                Entities[y * height + x].SetColor(new Color(Color.White, 0));
                drawPause.Reset();
            }

            drawPause.Update(gameTime);
        }

        private static void SetCurrentNumber()
        {
            if (input.KeyState.IsKeyDown(Keys.D1) && input.KeyReleased)
            {
                currentNumber = 1;
                input.KeyReleased = false;
            }
            if (input.KeyState.IsKeyDown(Keys.D2) && input.KeyReleased)
            {
                currentNumber = 2;
                input.KeyReleased = false;
            }
            if (input.KeyState.IsKeyDown(Keys.D3) && input.KeyReleased)
            {
                currentNumber = 3;
                input.KeyReleased = false;
            }
            if (input.KeyState.IsKeyDown(Keys.D4) && input.KeyReleased)
            {
                currentNumber = 4;
                input.KeyReleased = false;
            }
            if (input.KeyState.IsKeyDown(Keys.D5) && input.KeyReleased)
            {
                currentNumber = 5;
                input.KeyReleased = false;
            }
            if (input.KeyState.IsKeyDown(Keys.D6) && input.KeyReleased)
            {
                currentNumber = 6;
                input.KeyReleased = false;
            }
            if (input.KeyState.IsKeyDown(Keys.D7) && input.KeyReleased)
            {
                currentNumber = 7;
                input.KeyReleased = false;
            }
            if (input.KeyState.IsKeyDown(Keys.D8) && input.KeyReleased)
            {
                currentNumber = 8;
                input.KeyReleased = false;
            }
            if (input.KeyState.IsKeyDown(Keys.D9) && input.KeyReleased)
            {
                currentNumber = 9;
                input.KeyReleased = false;
            }
            if (input.KeyState.IsKeyDown(Keys.D0) && input.KeyReleased)
            {
                currentNumber = 0;
                input.KeyReleased = false;
            }
        }

        private static void SaveNumber(int number)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(number + " ");
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < height; x++)
                {
                    stringBuilder.Append(Entities[y * height + x].ObjectColor.A / 255 + " ");
                }
            }

            for (int x = 0; x < 10; x++)
            {
                if (x == number)
                {
                    stringBuilder.Append("1 ");
                }
                else
                {
                    stringBuilder.Append("0 ");
                }
            }

            Console.Clear();
            try
            {
                File.AppendAllText("Data/data.txt", stringBuilder.ToString() + "\n");
            }
            catch (IOException)
            {
                Console.WriteLine("Error.\n");
                return;
            }
            Console.WriteLine("Saved Successfully!\n");

            ResetBoard();
            DataStatistics();
        }

        private static int Total(int number, List<int> numbers)
        {
            int result = 0;
            foreach (int i in numbers)
            {
                result = number == i ? ++result : result;
            }
            return result;
        }

        private static void DataStatistics()
        {
            Console.WriteLine("Current Number: {0} \n", currentNumber);
            string[] data = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "Data/data.txt");
            List<int> numbers = new List<int>();
            foreach (string s in data)
            {
                numbers.Add(int.Parse(s.Substring(0, 1)));
            }
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("Total {0}'s : {1}", i, Total(i, numbers));
            }
            Console.WriteLine();
        }        

        private static void TrainNetwork()
        {
            Data data;
            // Train the network with random known data.
            for (int i = 0; i < 100; i++)
            {
                data = savedData[RNG.Next(0, savedData.Count)];
                neuralNetwork.Backpropagation(data.Input, data.Output);
            }
        }

        private static bool Empty()
        {
            foreach (Entity e in Entities)
            {
                if (e.ObjectColor.A != 0)
                {
                    return false;
                }
            }
            return true;
        }

        private static int MostConfidentPrediction(Matrix<double> output)
        {
            int result = 0;
            for (int i = 0; i < output.RowCount; i++)
            {
                if (output[i, 0] > output[result, 0])
                {
                    result = i;
                }
            }
            //Console.WriteLine(output[result, 0]);

            if (output[result, 0] >= 0.95)
                confidenceLevel = 3;
            else if (output[result, 0] >= 0.80)
                confidenceLevel = 2;
            else if (output[result, 0] >= 0.25)
                confidenceLevel = 1;
            else
                confidenceLevel = 0;

            return result;
        }

        private static void PredictNumber()
        {
            if (Empty() || !drawPause.Done)
            {
                text.SetText("This looks like the number ...");
                return;
            }

            double[] input = new double[21 * 21];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < height; x++)
                {
                    input[y * height + x] = Entities[y * height + x].ObjectColor.A / 255.0;
                }
            }

            guess = MostConfidentPrediction(neuralNetwork.Predict(input));

            switch (confidenceLevel) {
                case 3:
                    text.SetText("This looks like the number " + guess);
                    break;
                case 2:
                    text.SetText("This looks like the number " + guess + "?");
                    break;
                case 1:
                    text.SetText("This looks like the number " + guess + "??");
                    break;
                case 0:
                    text.SetText("I'm not really sure what that is");
                    break;
            }               
        }
        
        private static void UpdateEntities(GameTime gameTime)
        {
            for (int i = Entities.Count - 1; i >= 0; i--)
            {
                Entities[i].Update(gameTime);

                if (Entities[i].Remove)
                    Entities.RemoveAt(i);
            }

            if (EntityBuffer.Count > 0)
            {
                foreach (Entity e in EntityBuffer)
                    Entities.Add(e);

                EntityBuffer.Clear();
            }
        }

        public static void Update(GameTime gameTime)
        {
            UpdateInput(gameTime);
            CameraHandler(gameTime);

            if (train)
            {
                TrainNetwork();
            }
            else
            {
                PredictNumber();
            }          

            UpdateEntities(gameTime);
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, Camera.Transform);
            {
                foreach (Entity e in Entities)
                {
                    e.Draw(spriteBatch);
                }
                outline.Draw(spriteBatch);
                text.Draw(spriteBatch);
            }
            spriteBatch.End();

            if (train)
            {
                neuralNetwork.Draw(spriteBatch);
            }
        }
    }
}
