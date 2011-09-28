//using System;
//using System.Collections.Generic;
//using Microsoft.Xna.Framework;
//using pr2.Common;

//namespace pr2.CarotEngine.Paths {

//    ////goal: the pick up, rotate, and drop operation
//    ////1. pick up in a sort of smooth sine function (integrate sin -> h(t) = 1-cos(t)
//    ////2. rotate with a taper
//    ////3. plop down abruptly (maybe parabolic)

//    /// <summary>
//    /// The main function api. this takes a length parameter.. if you dont want it,
//    /// use FunctionBase and override eval(float t)
//    /// </summary>
//    public interface IFunction {
//        float eval(float t, float length);
//    }

//    public class ConstantFunction : IFunction {
//        float c;
//        public ConstantFunction(float c) { this.c = c; }
//        public float eval(float t, float length) { return c; }
//    }

//    /// <summary>
//    /// LF(t) = a*f(t) + c
//    /// </summary>
//    public class LinearFunction : IFunction {
//        IFunction f;
//        float a, c;
//        public LinearFunction(float a, float c, IFunction f) { this.f = f; this.a = a; this.c = c; }
//        public float eval(float t, float length) { return f.eval(t, length) * a + c; }
//    }

//    /// <summary>
//    /// Youll need to override one or the other eval() methods. otherwise theyll call each other ad infinitum
//    /// </summary>
//    public class FunctionBase : IFunction {

//        public FunctionBase() {}
		
//        public virtual float eval(float t, float length) { return eval(t); }
//        public virtual float eval(float t) { return eval(t, 0); }
//    }

//    public class TimedFunctionBase : FunctionBase {
//        public int length;

//        /// <summary>
//        /// Creates a function with a fixed length.
//        /// Of course, theres nothing to prevent you from scheduling it for longer....
//        /// </summary>
//        public TimedFunctionBase(int length) { this.length = length; }
//    }


//    public delegate float FunctionEvaluator(float t);
//    public delegate float TimedFunctionEvaluator(float t, float length);

//    public class DelegateFunction : IFunction {
//        public DelegateFunction(FunctionEvaluator eval) { this.evaluator = eval; }
//        FunctionEvaluator evaluator;
//        public float eval(float t, float length) { return evaluator(t); }
//    }

//    public class TimedDelegateFunction : IFunction {
//        public TimedDelegateFunction(TimedFunctionEvaluator eval) { this.evaluator = eval; }
//        TimedFunctionEvaluator evaluator;
//        public float eval(float t, float length) { return evaluator(t, length); }
//    }

//    public class Schedule {

//        //todo: indeterminate-time tracks? for when we dont want to calculate things
//        //actually theres no reason we couldnt have an ITrack and have different things.. 
//        //maybe unscheduled (just adjacent) things

//        class Track {

//            class Span : IComparable<Span> {
//                public Span(IFunction function, int start, int end) {
//                    this.function = function;
//                    this.start = start;
//                    this.end = end;
//                    length = end - start + 1;
//                }
//                //end is inclusive
//                public int start, end, length;
//                public IFunction function;

//                public int CompareTo(Span other) { return start - other.start; }
//            }

//            List<Span> spans = new List<Span>();

//            public void schedule(IFunction function, int start, int end) {
//                spans.Add(new Span(function, start, end));
//                spans.Sort();
//            }

//            public float defaultValue = 0;

//            public float eval(int time) {
//                for(int i = 0; i < spans.Count; i++)
//                    if(time >= spans[i].start && time <= spans[i].end)
//                        return spans[i].function.eval(time - spans[i].start, spans[i].length);
//                return defaultValue;
//            }

//            public bool isDoneAt(int time) {
//                if(spans.Count == 0) return true;
//                return time > spans[spans.Count - 1].end;
//            }
//        }

//        Dictionary<object, Track> tracks = new Dictionary<object, Track>();
//        Track someTrack; //some track. helpful only if you have one track
//        int currTime = 0;

//        void guarantee(object track) {
//            if(!tracks.ContainsKey(track))
//                tracks[track] = someTrack = new Track();
//        }

//        public void schedule(object track, int start, int length, IFunction function) {
//            guarantee(track);
//            tracks[track].schedule(function, start, start + length - 1);
//        }

//        public void schedule(object track, int start, int length, FunctionEvaluator function) {
//            schedule(track, start, length, new DelegateFunction(function));
//        }

//        public void schedule(object track, int start, int length, TimedFunctionEvaluator function) {
//            schedule(track, start, length, new TimedDelegateFunction(function));
//        }

//        /// <summary>
//        /// schedules a timed function. there is one less timing arg since the length is implied by the function
//        /// </summary>
//        public void schedule(object track, int start, TimedFunctionBase function) {
//            schedule(track, start, start + function.length, function);
//        }

//        public void setDefault(object track, float value) { guarantee(track); tracks[track].defaultValue = value; }
//        public float eval(object track, int time) { return tracks[track].eval(time); }
//        public float eval(object track) { return tracks[track].eval(currTime); }

//        class TriggerRecord {
//            public int minimumTime;
//            public CallbackArgReturnsR<float, bool> check;
//            public Callback callback;
//        }

//        public void defineTrigger(object track, int minimumTime, CallbackArgReturnsR<float, bool> check, Callback callback) {
//            TriggerRecord tr = new TriggerRecord();
//            tr.minimumTime = minimumTime;
//            tr.check = check;
//            tr.callback = callback;
//            triggers[tracks[track]].Add(tr);
//        }

//        Bag<object, TriggerRecord> triggers = new Bag<object, TriggerRecord>();
//        bool isEnded = false;

//        /// <summary>
//        /// evals a track. only makes sense if you have but one track. undefined if you have more than one
//        /// </summary>
//        public float eval() { return someTrack.eval(currTime); }

//        /// <summary>
//        /// returns true if each track is done playing
//        /// </summary>
//        public bool isDone() {
//            if(isEnded) return true;
//            foreach(Track track in tracks.Values)
//                if(!track.isDoneAt(currTime)) return false;
//            return true;
//        }

//        /// <summary>
//        /// evaluates each track and returns the sum
//        /// </summary>
//        public float sum() {
//            float ret = 0;
//            foreach(Track track in tracks.Values)
//                ret += track.eval(currTime);
//            return ret;
//        }

//        /// <summary>
//        /// evaluates each track and returns the product
//        /// </summary>
//        public float product() {
//            float ret = 1;
//            foreach(Track track in tracks.Values)
//                ret *= track.eval(currTime);
//            return ret;
//        }
//        public float product(int time) {
//            float ret = 1;
//            foreach(Track track in tracks.Values)
//                ret *= track.eval(currTime);
//            return ret;
//        }

//        public void tick(int time) { 
//            currTime += time;
//            foreach(Track track in tracks.Values)
//                foreach(TriggerRecord tr in triggers[track])
//                    if(tr.minimumTime <= currTime)
//                        if(tr.check(track.eval(currTime)))
//                            tr.callback();
//        }

//        /// <summary>
//        /// tells the schedule that it is done
//        /// </summary>
//        public void end() { isEnded = true; }

//    }

//    /// <summary>
//    /// creates a quarter cos pattern over the provided duration
//    /// </summary>
//    public class CosPattern : TimedFunctionBase {

//        public CosPattern(int length, float angle)
//            : base(length) {
//            cos = new CosFunction(angle/length);
//        }

//        public override float eval(float t) {
//            return cos.eval(t);
//        }

//        CosFunction cos;
//    }

//    /// <summary>
//    /// f(x) = ax^2 + bx + c
//    /// </summary>
//    public class QuadraticFunction : FunctionBase {
//        float a, b, c;
//        public QuadraticFunction(float a, float b, float c) { this.a = a; this.b = b; this.c = c; }
//        public override float eval(float t) {
//            return a * t * t + b * t + c;
//        }
//    }

//    /// <summary>
//    /// f(x) = d = s + vit + 1/2at^2 etc.
//    /// </summary>
//    public class BallisticFunction : QuadraticFunction {
//        float vi, a, s;
//        public BallisticFunction(float a, float vi, float s) : base(0.5f * a, vi, s) { }
//    }

//    /// <summary>
//    /// Returns the max of two functions
//    /// </summary>
//    public class MaxFunction : IFunction {
//        IFunction f, g;
//        public MaxFunction(IFunction f, IFunction g) { this.f = f; this.g = g; }
//        /// <summary>
//        /// one of the functions is the constant function
//        /// </summary>
//        public MaxFunction(float c, IFunction f) { this.f = f; this.g = new ConstantFunction(c); }
//        public float eval(float t, float length) { return Math.Max(f.eval(t, length), g.eval(t, length)); }
//    }


//    /// <summary>
//    /// a decaying exponential function
//    /// </summary>
//    public class ExpDecayFunction : FunctionBase {
//        float coef, decay;
//        public ExpDecayFunction(float coef, float decay) { this.coef = coef; this.decay = decay; }
//        public ExpDecayFunction(float decay) { this.decay = decay; }
//        public override float eval(float t) {
//            return coef * (1-(float)Math.Exp(-t * decay));
//        }
//    }

//    public class HalfCosPattern : CosPattern {
//        public HalfCosPattern(int length) : base(length, (float)Math.PI) { }
//    }

//    public class CosFunction : SinFunction {
//        public CosFunction(float omega, float phase) : base(omega, phase + (float)Math.PI / 2) { }
//        public CosFunction(float omega) : base(omega, (float)Math.PI / 2) { }
//    }

//    public class SinFunction : FunctionBase {
//        float omega, phase = 0;
//        public SinFunction(float omega, float phase) { this.omega = omega; this.phase = phase; }
//        public SinFunction(float omega) { this.omega = omega; }
//        public override float eval(float t) { return (float)Math.Sin(omega * t + phase); }
//    }


//}