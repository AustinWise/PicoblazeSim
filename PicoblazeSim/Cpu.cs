using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Austin.PicoblazeSim
{

    /// <summary>
    /// Represents a PicoBlaze processor.
    /// </summary>
    public class Cpu
    {
        /// <summary>
        /// Creates a new instance of the CPU with the given instruction memory.
        /// </summary>
        /// <param name="iMem">instruction memory, mapping memory location to instruction</param>
        public Cpu(Dictionary<ushort, uint> iMem)
        {
            this.instructionMemory = new uint[0x400];
            for (ushort i = 0; i < 0x400; i++)
            {
                if (iMem.ContainsKey(i))
                    instructionMemory[i] = iMem[i];
            }
            this.state = new CpuState();
            this.SleepTickInterval = DefaultSleepTickInterval;
        }

        private uint[] instructionMemory;
        private InstructionFactory ops = new InstructionFactory();
        private bool isRunning = false;
        private List<IHardwareDevice> devices = new List<IHardwareDevice>();
        private CpuState state;
        private bool interruptFlag = false;
        private Thread runThread;
        
        private int stallCounter = 0;
        private long tickCount;
        private DateTime startTime;
        private DateTime endTime;

        public void RegisterHardwareDevice(IHardwareDevice dev)
        {
            devices.Add(dev);
            hookupDevice(dev);
            if (dev is IInterruptor)
                ((IInterruptor)dev).Interrupt += new EventHandler(dev_Interrupt);
        }

        void dev_Interrupt(object sender, EventArgs e)
        {
            this.Interrupt();
        }

        private void hookupDevice(IHardwareDevice dev)
        {
            foreach (var input in dev.InputDevices)
            {
                state.InputDevices.Add(input.Key, input.Value);
            }
            foreach (var output in dev.OutputDevices)
            {
                state.OutputDevices.Add(output.Key, output.Value);
            }
        }

        /// <summary>
        /// States the CPU precessing insructions as fast as it can on a seperate thread.
        /// </summary>
        public void Start()
        {
            if (isRunning)
                return;
            if (runThread != null && runThread.IsAlive)
                runThread.Join();
            isRunning = true;
            runThread = new Thread(new ThreadStart(loop));
            runThread.Start();
        }

        private void loop()
        {
            tickCount = 0;
            startTime = DateTime.Now;
            while (isRunning)
            {
                Tick();
            }
            this.endTime = DateTime.Now;
        }


        /// <summary>
        /// Executes one instruction.
        /// </summary>
        public void Tick()
        {
            if (interruptFlag)
            {
                interruptFlag = false;
                this.state.EnableInterrupts = false;
                this.state.InterruptedProgramCounter = this.state.ProgramCounter;
                this.state.ProgramCounter = 0x3ff;
            }
            uint instruction = this.instructionMemory[state.ProgramCounter];
            byte opCode = (byte)(instruction >> 12);
            ushort args = (ushort)(0XFFF & instruction);
            ops.Get(opCode).Do(state, args);

            tickCount++;

            // stall the processor to reach a target execution rate
            stallCounter++;
            if (stallCounter == sleepTickInterval)
            {
                Thread.Sleep(1);
                stallCounter = 0;
            }
        }

        /// <summary>
        /// Signals an interrupt.
        /// </summary>
        public void Interrupt()
        {
            if (state.EnableInterrupts)
                this.interruptFlag = true;
        }

        /// <summary>
        /// Stops and resets the processor.  
        /// </summary>
        /// <returns>The the instructions per second executed.</returns>
        public double Reset()
        {
            this.isRunning = false;
            runThread.Join();
            this.state = new CpuState();
            foreach (var dev in devices)
            {
                hookupDevice(dev);
            }
            return tickCount / (endTime.Subtract(startTime).TotalSeconds);
        }

        public bool IsRunning
        {
            get
            {
                return this.isRunning;
            }
        }

        public const int DefaultSleepTickInterval = 25000;

        private int sleepTickInterval;
        /// <summary>
        /// Sleep the execution thread for 1 millisecond
        /// after this many instructions have been executed.
        /// </summary>
        public int SleepTickInterval
        {
            get
            {
                return sleepTickInterval;
            }
            set
            {
                this.sleepTickInterval = value;
            }
        }
    }
}
